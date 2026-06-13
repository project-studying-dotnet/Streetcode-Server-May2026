using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Streetcode.BLL.Interfaces.WebParsingUtils;
using Streetcode.DAL.Entities.Toponyms;
using Streetcode.DAL.Persistence;

namespace Streetcode.BLL.Services.WebParsingUtils;

public class ToponymDataService : IToponymData
{
    private readonly StreetcodeDbContext _context;
    private readonly ILogger<ToponymDataService> _logger;

    public ToponymDataService(StreetcodeDbContext context, ILogger<ToponymDataService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task RefreshToponymsInDbAsync(List<Toponym> validToponyms)
    {
        _logger.LogInformation("Updating database with valid toponyms...");
        await using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            _context.Toponyms.RemoveRange(_context.Toponyms);
            await _context.SaveChangesAsync();

            await _context.Toponyms.AddRangeAsync(validToponyms);
            await _context.SaveChangesAsync();

            await transaction.CommitAsync();
            _logger.LogInformation("Toponyms successfully updated in the database.");
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            _logger.LogError(ex, "Database transaction error. Rolling back changes.");
            throw;
        }
    }
}