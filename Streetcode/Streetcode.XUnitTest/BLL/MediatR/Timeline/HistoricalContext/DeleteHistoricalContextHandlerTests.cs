using System.Linq.Expressions;
using Moq;
using Xunit;
using AutoMapper;
using FluentResults;
using FluentAssertions;
using Streetcode.BLL.Resources;
using Streetcode.BLL.DTO.Timeline;
using Streetcode.BLL.Interfaces.Logging;
using Microsoft.EntityFrameworkCore.Query;
using Streetcode.DAL.Repositories.Interfaces.Base;
using Streetcode.DAL.Repositories.Interfaces.Timeline;
using Streetcode.BLL.MediatR.Timeline.HistoricalContext;
using HistContext = Streetcode.DAL.Entities.Timeline.HistoricalContext;

namespace Streetcode.XUnitTest.BLL.MediatR.Timeline.HistoricalContext;

public sealed class DeleteHistoricalContextHandlerTests
{
    private Mock<IRepositoryWrapper> RepositoryWrapperMock { get; }
    private Mock<IHistoricalContextRepository> HistoricalContextRepositoryMock { get; }
    private Mock<ILoggerService> LoggerMock { get; }
    private IMapper Mapper { get; }
    private DeleteHistoricalContextHandler Handler { get; }

    public DeleteHistoricalContextHandlerTests()
    {
        RepositoryWrapperMock = new Mock<IRepositoryWrapper>();
        HistoricalContextRepositoryMock = new Mock<IHistoricalContextRepository>();
        RepositoryWrapperMock.Setup(r => r.HistoricalContextRepository).Returns(HistoricalContextRepositoryMock.Object);

        LoggerMock = new Mock<ILoggerService>();
        MapperConfiguration config = new(cfg =>
        {
            cfg.AddMaps(typeof(DeleteHistoricalContextHandler).Assembly);
        });
        Mapper = config.CreateMapper();
        Handler = new DeleteHistoricalContextHandler(RepositoryWrapperMock.Object, Mapper, LoggerMock.Object);
    }

    [Fact]
    public async Task Handle_ReturnsHistoricalContext_WhenDeletedSuccessfully()
    {
        // Arrange
        HistoricalContextDto dto = new()
        {
            Id = 1,
            Title = "Test Historical Context"
        };
        HistContext context = Mapper.Map<HistContext>(dto);
        DeleteHistoricalContextCommand command = new(dto.Id);
        HistoricalContextRepositoryMock.Setup(
            r => r.GetFirstOrDefaultAsync(
                It.IsAny<Expression<Func<HistContext, bool>>>(),
                It.IsAny<Func<IQueryable<HistContext>, IIncludableQueryable<HistContext, object>>>(),
                It.IsAny<CancellationToken>()
            )
        ).ReturnsAsync(context);
        HistoricalContextRepositoryMock.Setup(r => r.Delete(context));
        RepositoryWrapperMock.Setup(
            r => r.SaveChangesAsync(It.IsAny<CancellationToken>())
        ).ReturnsAsync(1);

        // Act
        Result<HistoricalContextDto> result = await Handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        dto.Should().BeEquivalentTo(result.Value);
        HistoricalContextRepositoryMock.Verify(
            r => r.GetFirstOrDefaultAsync(
                It.IsAny<Expression<Func<HistContext, bool>>>(),
                It.IsAny<Func<IQueryable<HistContext>, IIncludableQueryable<HistContext, object>>>(),
                It.IsAny<CancellationToken>()
            ),
            Times.Once
        );
        HistoricalContextRepositoryMock.Verify(r => r.Delete(context), Times.Once);
        RepositoryWrapperMock.Verify(
            r => r.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Once
        );
    }

    [Fact]
    public async Task Handle_ReturnsError_WhenDeleteFails()
    {
        // Arrange
        string error_msg = string.Format(ErrorMessages.FailedToDeleteType, nameof(HistContext));
        HistoricalContextDto dto = new()
        {
            Id = 1,
            Title = "Test Historical Context"
        };
        HistContext context = Mapper.Map<HistContext>(dto);
        DeleteHistoricalContextCommand command = new(dto.Id);
        HistoricalContextRepositoryMock.Setup(
            r => r.GetFirstOrDefaultAsync(
                It.IsAny<Expression<Func<HistContext, bool>>>(),
                It.IsAny<Func<IQueryable<HistContext>, IIncludableQueryable<HistContext, object>>>(),
                It.IsAny<CancellationToken>()
            )
        ).ReturnsAsync(context);
        HistoricalContextRepositoryMock.Setup(r => r.Delete(context));
        RepositoryWrapperMock.Setup(
            r => r.SaveChangesAsync(It.IsAny<CancellationToken>())
        ).ReturnsAsync(0);
        LoggerMock.Setup(l => l.LogError(command, error_msg));

        // Act
        Result<HistoricalContextDto> result = await Handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().BeEquivalentTo([
            new Error(error_msg)
        ]);
        HistoricalContextRepositoryMock.Verify(
            r => r.GetFirstOrDefaultAsync(
                It.IsAny<Expression<Func<HistContext, bool>>>(),
                It.IsAny<Func<IQueryable<HistContext>, IIncludableQueryable<HistContext, object>>>(),
                It.IsAny<CancellationToken>()
            ),
            Times.Once
        );
        HistoricalContextRepositoryMock.Verify(r => r.Delete(context), Times.Once);
        RepositoryWrapperMock.Verify(
            r => r.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Once
        );
        LoggerMock.Verify(l => l.LogError(command, error_msg), Times.Once);
    }

    [Fact]
    public async Task Handle_ReturnsError_WhenHistoricalContextNotFound()
    {
        // Arrange
        HistoricalContextDto dto = new()
        {
            Id = 1,
            Title = "Test Historical Context"
        };
        string error_msg = string.Format(ErrorMessages.TypeWithIdNotFound, nameof(HistContext), dto.Id);
        HistContext context = Mapper.Map<HistContext>(dto);
        DeleteHistoricalContextCommand command = new(dto.Id);
        HistoricalContextRepositoryMock.Setup(
            r => r.GetFirstOrDefaultAsync(
                It.IsAny<Expression<Func<HistContext, bool>>>(),
                It.IsAny<Func<IQueryable<HistContext>, IIncludableQueryable<HistContext, object>>>(),
                It.IsAny<CancellationToken>()
            )
        ).ReturnsAsync((HistContext?)null);
        LoggerMock.Setup(l => l.LogError(command, error_msg));

        // Act
        Result<HistoricalContextDto> result = await Handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().BeEquivalentTo([
            new Error(error_msg)
        ]);
        HistoricalContextRepositoryMock.Verify(
            r => r.GetFirstOrDefaultAsync(
                It.IsAny<Expression<Func<HistContext, bool>>>(),
                It.IsAny<Func<IQueryable<HistContext>, IIncludableQueryable<HistContext, object>>>(),
                It.IsAny<CancellationToken>()
            ),
            Times.Once
        );
        LoggerMock.Verify(l => l.LogError(command, error_msg), Times.Once);
    }
}