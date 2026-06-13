#nullable disable

using System.Linq.Expressions;
using AutoMapper;
using FluentAssertions;
using Microsoft.EntityFrameworkCore.Query;
using Moq;
using Streetcode.BLL.DTO.Streetcode;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.MediatR.Streetcode.Streetcode.GetAllMainPage;
using Streetcode.BLL.MediatR.Streetcode.Streetcode.GetAllStreetcodesMainPage;
using Streetcode.DAL.Entities.Streetcode;
using Streetcode.DAL.Repositories.Interfaces.Base;
using Xunit;

namespace Streetcode.XUnitTest.BLL.MediatR.StreetCode.Streetcode.GetAllMainPage;

public class GetAllStreetcodesMainPageHandlerTests
{
    private readonly Mock<IRepositoryWrapper> _mockRepositoryWrapper;
    private readonly IMapper _mapper;
    private readonly Mock<ILoggerService> _mockLogger;
    private readonly GetAllStreetcodesMainPageHandler _handler;

    public GetAllStreetcodesMainPageHandlerTests()
    {
        _mockRepositoryWrapper = new Mock<IRepositoryWrapper>();
        _mapper = new MapperConfiguration(opt =>
        {
            opt.AddMaps(typeof(GetAllStreetcodesMainPageHandler).Assembly);
        }).CreateMapper();
        _mockLogger = new Mock<ILoggerService>();

        _handler = new GetAllStreetcodesMainPageHandler(
            _mockRepositoryWrapper.Object,
            _mapper,
            _mockLogger.Object);
    }

    [Fact]
    public async Task Handle_WhenStreetcodesExist_ReturnsOkResultWithMappedDtos()
    {
        var streetcodes = new List<StreetcodeContent> { new StreetcodeContent { Id = 1 } };
        var mappedDtos = _mapper.Map<IEnumerable<StreetcodeMainPageDTO>>(streetcodes);
        var request = new GetAllStreetcodesMainPageQuery();

        _mockRepositoryWrapper.Setup(r => r.StreetcodeRepository.GetAllAsync(
            It.IsAny<Expression<Func<StreetcodeContent, bool>>>(),
            It.IsAny<Func<IQueryable<StreetcodeContent>, IIncludableQueryable<StreetcodeContent, object>>>()))
            .ReturnsAsync(streetcodes);

        var result = await _handler.Handle(request, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEquivalentTo(mappedDtos);
    }

    [Fact]
    public async Task Handle_WhenStreetcodesAreNull_ReturnsFailResultAndLogsError()
    {
        var request = new GetAllStreetcodesMainPageQuery();
        IEnumerable<StreetcodeContent> nullStreetcodes = null;

        _mockRepositoryWrapper.Setup(r => r.StreetcodeRepository.GetAllAsync(
            It.IsAny<Expression<Func<StreetcodeContent, bool>>>(),
            It.IsAny<Func<IQueryable<StreetcodeContent>, IIncludableQueryable<StreetcodeContent, object>>>()))
            .ReturnsAsync(nullStreetcodes);

        var result = await _handler.Handle(request, CancellationToken.None);

        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainSingle(e => e.Message == "No streetcodes exist now");

        _mockLogger.Verify(
            l => l.LogError(request, "No streetcodes exist now"),
            Times.Once);
    }
}