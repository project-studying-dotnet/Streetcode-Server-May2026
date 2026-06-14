using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Logging;
using Moq;
using Streetcode.BLL.Services.WebParsingUtils;
using Streetcode.DAL.Entities.Toponyms;
using Streetcode.DAL.Persistence;
using Xunit;

namespace Streetcode.XUnitTest.BLL.Services.WebParsingUtils;

public class ToponymDataServiceTests : IDisposable
{
    private readonly StreetcodeDbContext _context;
    private readonly Mock<ILogger<ToponymDataService>> _loggerMock;

    public ToponymDataServiceTests()
    {
        _loggerMock = new Mock<ILogger<ToponymDataService>>();

        var options = new DbContextOptionsBuilder<StreetcodeDbContext>()
            .UseInMemoryDatabase(databaseName: $"Streetcode_Toponym_Tests_{Guid.NewGuid()}")
            .ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning))
            .Options;

        _context = new StreetcodeDbContext(options);
        _context.Database.EnsureCreated();
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
        GC.SuppressFinalize(this);
    }

    [Fact]
    public async Task RefreshToponymsInDbAsync_WithValidData_ClearsOldAndAddsNewToponyms()
    {
        var oldToponyms = new List<Toponym>
        {
            new Toponym { Id = 1, StreetName = "Стара Вулиця", Oblast = "Львівська" },
            new Toponym { Id = 2, StreetName = "Колишній Проспект", Oblast = "Київська" }
        };
        await _context.Toponyms.AddRangeAsync(oldToponyms);
        await _context.SaveChangesAsync();

        var service = new ToponymDataService(_context, _loggerMock.Object);

        var newToponyms = new List<Toponym>
        {
            new Toponym { Id = 3, StreetName = "Нова Вулиця", Oblast = "Львівська" }
        };

        await service.RefreshToponymsInDbAsync(newToponyms);

        var currentToponyms = await _context.Toponyms.ToListAsync();

        Assert.Single(currentToponyms);
        Assert.Equal("Нова Вулиця", currentToponyms[0].StreetName);
        Assert.DoesNotContain(currentToponyms, t => t.StreetName == "Стара Вулиця");
    }

    [Fact]
    public async Task RefreshToponymsInDbAsync_WhenExceptionOccurs_ThrowsInvalidOperationExceptionWithInnerException()
    {
        var initialToponyms = new List<Toponym>
        {
            new Toponym { Id = 1, StreetName = "Незмінна Вулиця", Oblast = "Одеська" }
        };
        await _context.Toponyms.AddRangeAsync(initialToponyms);
        await _context.SaveChangesAsync();

        var service = new ToponymDataService(_context, _loggerMock.Object);

        var invalidToponyms = new List<Toponym> { null! };

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            service.RefreshToponymsInDbAsync(invalidToponyms));

        Assert.Contains("Failed to update toponyms and coordinates", exception.Message);
        Assert.NotNull(exception.InnerException);
    }
}