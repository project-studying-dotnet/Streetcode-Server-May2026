using System.Linq.Expressions;
using Moq;
using Xunit;
using AutoMapper;
using FluentResults;
using FluentAssertions;
using MockQueryable.Moq;
using Streetcode.BLL.Resources;
using Streetcode.BLL.DTO.Timeline;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.DAL.Repositories.Interfaces.Base;
using Streetcode.DAL.Repositories.Interfaces.Timeline;
using Streetcode.BLL.MediatR.Timeline.HistoricalContext;
using HistContext = Streetcode.DAL.Entities.Timeline.HistoricalContext;

namespace Streetcode.XUnitTest.BLL.MediatR.Timeline.HistoricalContext;

public sealed class GetAllHistoricalContextHandlerTests
{
    private Mock<IRepositoryWrapper> RepositoryWrapperMock { get; }
    private Mock<IHistoricalContextRepository> HistoricalContextRepositoryMock { get; }
    private Mock<ILoggerService> LoggerMock { get; }
    private IMapper Mapper { get; }
    private GetAllHistoricalContextHandler Handler { get; }

    public GetAllHistoricalContextHandlerTests()
    {
        RepositoryWrapperMock = new Mock<IRepositoryWrapper>();
        HistoricalContextRepositoryMock = new Mock<IHistoricalContextRepository>();
        RepositoryWrapperMock.Setup(r => r.HistoricalContextRepository).Returns(HistoricalContextRepositoryMock.Object);

        LoggerMock = new Mock<ILoggerService>();
        MapperConfiguration config = new(cfg =>
        {
            cfg.AddMaps(typeof(GetAllHistoricalContextHandler).Assembly);
        });
        Mapper = config.CreateMapper();
        Handler = new GetAllHistoricalContextHandler(RepositoryWrapperMock.Object, Mapper, LoggerMock.Object);
    }

    [Fact]
    public async Task Handle_ReturnsHistoricalContexts_WhenFoundNotEmpty()
    {
        // Arrange
        List<HistContext> historical_contexts = [
            new HistContext()
            {
                Id = 1,
                Title = "Historical Context 1"
            },
            new HistContext()
            {
                Id = 2,
                Title = "Historical Context 2"
            }
        ];
        List<HistoricalContextDto> dtos = Mapper.Map<IEnumerable<HistoricalContextDto>>(historical_contexts).ToList();
        GetAllHistoricalContextQuery query = new();
        HistoricalContextRepositoryMock.Setup(
            r => r.FindAll(It.IsAny<Expression<Func<HistContext, bool>>>())
        ).Returns(historical_contexts.BuildMock());

        // Act
        Result<IEnumerable<HistoricalContextDto>> result = await Handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEquivalentTo(dtos);
        HistoricalContextRepositoryMock.Verify(
            r => r.FindAll(It.IsAny<Expression<Func<HistContext, bool>>>()),
            Times.Once
        );
    }

    [Fact]
    public async Task Handle_ReturnsError_WhenFoundEmpty()
    {
        // Arrange
        string error_msg = string.Format(ErrorMessages.FailedToFindAnyType, nameof(HistContext));
        List<HistContext> historical_contexts = [];
        List<HistoricalContextDto> dtos = Mapper.Map<IEnumerable<HistoricalContextDto>>(historical_contexts).ToList();
        GetAllHistoricalContextQuery query = new();
        HistoricalContextRepositoryMock.Setup(
            r => r.FindAll(It.IsAny<Expression<Func<HistContext, bool>>>())
        ).Returns(historical_contexts.BuildMock());
        LoggerMock.Setup(l => l.LogError(query, error_msg));

        // Act
        Result<IEnumerable<HistoricalContextDto>> result = await Handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().BeEquivalentTo([
            new Error(error_msg)
        ]);
    }
}