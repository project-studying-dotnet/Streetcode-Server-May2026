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
using Streetcode.BLL.MediatR.Timeline.HistoricalContext.Update;
using HistContext = Streetcode.DAL.Entities.Timeline.HistoricalContext;

namespace Streetcode.XUnitTest.BLL.MediatR.Timeline.HistoricalContext;

public sealed class UpdateHistoricalContextHandlerTests
{
    private readonly Mock<IRepositoryWrapper> _mockRepositoryWrapper;
    private readonly Mock<IHistoricalContextRepository> _mockHistoricalContextRepository;
    private readonly Mock<ILoggerService> _mockLogger;
    private readonly IMapper _mapper;
    private readonly UpdateHistoricalContextHandler _handler;

    public UpdateHistoricalContextHandlerTests()
    {
        _mockRepositoryWrapper = new Mock<IRepositoryWrapper>();
        _mockHistoricalContextRepository = new Mock<IHistoricalContextRepository>();
        _mockRepositoryWrapper.Setup(r => r.HistoricalContextRepository).Returns(_mockHistoricalContextRepository.Object);

        _mockLogger = new Mock<ILoggerService>();
        MapperConfiguration config = new(cfg =>
        {
            cfg.AddMaps(typeof(UpdateHistoricalContextHandler).Assembly);
        });
        _mapper = config.CreateMapper();
        _handler = new UpdateHistoricalContextHandler(_mockRepositoryWrapper.Object, _mapper, _mockLogger.Object);
    }

    [Fact]
    public async Task Handle_ReturnsUpdatedHistoricalContext_WhenUpdatedSuccessfully()
    {
        // Arrange
        HistoricalContextDto dto = new()
        {
            Id = 1,
            Title = "Test Historical Context"
        };
        HistContext context = _mapper.Map<HistContext>(dto);
        UpdateHistoricalContextCommand command = new(dto);
        _mockHistoricalContextRepository.Setup(
            r => r.GetFirstOrDefaultAsync(
                It.IsAny<Expression<Func<HistContext, bool>>>(),
                It.IsAny<Func<IQueryable<HistContext>, IIncludableQueryable<HistContext, object>>>(),
                It.IsAny<CancellationToken>()
            )
        ).ReturnsAsync(context);
        _mockHistoricalContextRepository.Setup(
            r => r.Update(context)
        );
        _mockRepositoryWrapper.Setup(
            r => r.SaveChangesAsync(It.IsAny<CancellationToken>())
        ).ReturnsAsync(1);

        // Act
        Result<HistoricalContextDto> result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        dto.Should().BeEquivalentTo(result.Value);
        _mockHistoricalContextRepository.Verify(
            r => r.GetFirstOrDefaultAsync(
                It.IsAny<Expression<Func<HistContext, bool>>>(),
                It.IsAny<Func<IQueryable<HistContext>, IIncludableQueryable<HistContext, object>>>(),
                It.IsAny<CancellationToken>()
            ),
            Times.Once
        );
        _mockHistoricalContextRepository.Verify(
            r => r.Update(context),
            Times.Once
        );
        _mockRepositoryWrapper.Verify(
            r => r.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Once
        );
    }

    [Fact]
    public async Task Handle_ReturnsError_WhenUpdateFails()
    {
        // Arrange
        HistoricalContextDto dto = new()
        {
            Id = 1,
            Title = "Test Historical Context"
        };
        HistContext context = _mapper.Map<HistContext>(dto);
        UpdateHistoricalContextCommand command = new(dto);
        _mockHistoricalContextRepository.Setup(
            r => r.GetFirstOrDefaultAsync(
                It.IsAny<Expression<Func<HistContext, bool>>>(),
                It.IsAny<Func<IQueryable<HistContext>, IIncludableQueryable<HistContext, object>>>(),
                It.IsAny<CancellationToken>()
            )
        ).ReturnsAsync(context);
        _mockHistoricalContextRepository.Setup(
            r => r.Update(context)
        );
        _mockRepositoryWrapper.Setup(
            r => r.SaveChangesAsync(It.IsAny<CancellationToken>())
        ).ReturnsAsync(0);
        _mockLogger.Setup(
            l => l.LogError(command, string.Format(ErrorMessages.FailedToUpdateHistoricalContextWithId, dto.Id))
        );

        // Act
        Result<HistoricalContextDto> result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().BeEquivalentTo([
            new Error(string.Format(ErrorMessages.FailedToUpdateHistoricalContextWithId, dto.Id))
        ]);
        _mockHistoricalContextRepository.Verify(
            r => r.GetFirstOrDefaultAsync(
                It.IsAny<Expression<Func<HistContext, bool>>>(),
                It.IsAny<Func<IQueryable<HistContext>, IIncludableQueryable<HistContext, object>>>(),
                It.IsAny<CancellationToken>()
            ),
            Times.Once
        );
        _mockHistoricalContextRepository.Verify(
            r => r.Update(context),
            Times.Once
        );
        _mockRepositoryWrapper.Verify(
            r => r.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Once
        );
        _mockLogger.Verify(
            l => l.LogError(command, string.Format(ErrorMessages.FailedToUpdateHistoricalContextWithId, dto.Id)),
            Times.Once
        );
    }

    [Fact]
    public async Task Handle_ReturnsError_WhenExceptionOccurs()
    {
        // Arrange
        const string exception_message = "Database exception";
        HistoricalContextDto dto = new()
        {
            Id = 1,
            Title = "Test Historical Context"
        };
        HistContext context = _mapper.Map<HistContext>(dto);
        UpdateHistoricalContextCommand command = new(dto);
        _mockHistoricalContextRepository.Setup(
            r => r.GetFirstOrDefaultAsync(
                It.IsAny<Expression<Func<HistContext, bool>>>(),
                It.IsAny<Func<IQueryable<HistContext>, IIncludableQueryable<HistContext, object>>>(),
                It.IsAny<CancellationToken>()
            )
        ).ReturnsAsync(context);
        _mockHistoricalContextRepository.Setup(
            r => r.Update(context)
        );
        _mockRepositoryWrapper.Setup(
            r => r.SaveChangesAsync(It.IsAny<CancellationToken>())
        ).ThrowsAsync(new Exception(exception_message));
        _mockLogger.Setup(
            l => l.LogError(command, exception_message)
        );

        // Act
        Result<HistoricalContextDto> result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().BeEquivalentTo([new Error(exception_message)]);
        _mockHistoricalContextRepository.Verify(
            r => r.GetFirstOrDefaultAsync(
                It.IsAny<Expression<Func<HistContext, bool>>>(),
                It.IsAny<Func<IQueryable<HistContext>, IIncludableQueryable<HistContext, object>>>(),
                It.IsAny<CancellationToken>()
            ),
            Times.Once
        );
        _mockHistoricalContextRepository.Verify(
            r => r.Update(context),
            Times.Once
        );
        _mockRepositoryWrapper.Verify(
            r => r.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Once
        );
        _mockLogger.Verify(
            l => l.LogError(command, exception_message),
            Times.Once
        );
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
        HistContext context = _mapper.Map<HistContext>(dto);
        UpdateHistoricalContextCommand command = new(dto);
        _mockHistoricalContextRepository.Setup(
            r => r.GetFirstOrDefaultAsync(
                It.IsAny<Expression<Func<HistContext, bool>>>(),
                It.IsAny<Func<IQueryable<HistContext>, IIncludableQueryable<HistContext, object>>>(),
                It.IsAny<CancellationToken>()
            )
        ).ReturnsAsync((HistContext?)null);
        _mockLogger.Setup(
            l => l.LogError(command, string.Format(ErrorMessages.HistoricalContextWithIdNotFound, dto.Id))
        );

        // Act
        Result<HistoricalContextDto> result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().BeEquivalentTo([
            new Error(string.Format(ErrorMessages.HistoricalContextWithIdNotFound, dto.Id))
        ]);
        _mockHistoricalContextRepository.Verify(
            r => r.GetFirstOrDefaultAsync(
                It.IsAny<Expression<Func<HistContext, bool>>>(),
                It.IsAny<Func<IQueryable<HistContext>, IIncludableQueryable<HistContext, object>>>(),
                It.IsAny<CancellationToken>()
            ),
            Times.Once
        );
        _mockLogger.Verify(
            l => l.LogError(command, string.Format(ErrorMessages.HistoricalContextWithIdNotFound, dto.Id)),
            Times.Once
        );
    }
}