using System.Linq.Expressions;
using FluentAssertions;
using Microsoft.EntityFrameworkCore.Query;
using Moq;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.MediatR.Streetcode.RelatedFigure.Create;
using Streetcode.BLL.Resources;
using Streetcode.DAL.Entities.Streetcode;
using Streetcode.DAL.Repositories.Interfaces.Base;
using Streetcode.DAL.Repositories.Interfaces.Streetcode;
using Xunit;

using RelatedFigureEntity = global::Streetcode.DAL.Entities.Streetcode.RelatedFigure;

namespace Streetcode.XUnitTest.BLL.MediatR.Streetcode.RelatedFigure.Create;

public class CreateRelatedFigureHandlerTests
{
    private const int ObserverId = 1;
    private const int TargetId = 2;
    private const string ObserverTitle = "Observer";
    private const string TargetTitle = "Target";
    private const string ObserverUrl = "observer";
    private const string TargetUrl = "target";

    private readonly Mock<IRepositoryWrapper> _repositoryWrapperMock;
    private readonly Mock<IStreetcodeRepository> _streetcodeRepositoryMock;
    private readonly Mock<IRelatedFigureRepository> _relatedFigureRepositoryMock;
    private readonly Mock<ILoggerService> _loggerMock;
    private readonly CreateRelatedFigureHandler _handler;

    public CreateRelatedFigureHandlerTests()
    {
        _repositoryWrapperMock = new Mock<IRepositoryWrapper>();
        _streetcodeRepositoryMock = new Mock<IStreetcodeRepository>();
        _relatedFigureRepositoryMock = new Mock<IRelatedFigureRepository>();
        _loggerMock = new Mock<ILoggerService>();

        _repositoryWrapperMock
            .Setup(wrapper => wrapper.StreetcodeRepository)
            .Returns(_streetcodeRepositoryMock.Object);

        _repositoryWrapperMock
            .Setup(wrapper => wrapper.RelatedFigureRepository)
            .Returns(_relatedFigureRepositoryMock.Object);

        _handler = new CreateRelatedFigureHandler(
            _repositoryWrapperMock.Object,
            null!,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnOk_WhenRelationCreated()
    {
        var command = new CreateRelatedFigureCommand(ObserverId, TargetId);

        SetupStreetcodeRepositorySequence(
            CreateStreetcode(ObserverId, ObserverTitle, ObserverUrl),
            CreateStreetcode(TargetId, TargetTitle, TargetUrl));

        _repositoryWrapperMock
            .Setup(wrapper => wrapper.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();

        _relatedFigureRepositoryMock.Verify(
            repository => repository.Create(
                It.Is<RelatedFigureEntity>(
                    relation => relation.ObserverId == ObserverId &&
                                relation.TargetId == TargetId)),
            Times.Once);

        _repositoryWrapperMock.Verify(
            wrapper => wrapper.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Once);

        _loggerMock.Verify(
            logger => logger.LogError(
                It.IsAny<object>(),
                It.IsAny<string>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldReturnFail_WhenObserverDoesNotExist()
    {
        var command = new CreateRelatedFigureCommand(ObserverId, TargetId);

        SetupStreetcodeRepositorySequence(null!, null!);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsFailed.Should().BeTrue();

        result.Errors.Should()
            .ContainSingle(error =>
                error.Message == string.Format(
                    ErrorMessages.NoExistingStreetcodeWithId,
                    ObserverId));

        _relatedFigureRepositoryMock.Verify(
            repository => repository.Create(It.IsAny<RelatedFigureEntity>()),
            Times.Never);

        _repositoryWrapperMock.Verify(
            wrapper => wrapper.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Never);

        _loggerMock.Verify(
            logger => logger.LogError(
                command,
                It.Is<string>(message => message.Contains(ObserverId.ToString()))),
            Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldReturnFail_WhenTargetDoesNotExist()
    {
        var command = new CreateRelatedFigureCommand(ObserverId, TargetId);

        SetupStreetcodeRepositorySequence(
            CreateStreetcode(ObserverId, ObserverTitle, ObserverUrl),
            null);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsFailed.Should().BeTrue();

        result.Errors.Should()
            .ContainSingle(error =>
                error.Message == string.Format(
                    ErrorMessages.NoExistingStreetcodeWithId,
                    TargetId));

        _relatedFigureRepositoryMock.Verify(
            repository => repository.Create(It.IsAny<RelatedFigureEntity>()),
            Times.Never);

        _repositoryWrapperMock.Verify(
            wrapper => wrapper.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Never);

        _loggerMock.Verify(
            logger => logger.LogError(
                command,
                It.Is<string>(message => message.Contains(TargetId.ToString()))),
            Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldReturnFail_WhenSaveChangesFails()
    {
        var command = new CreateRelatedFigureCommand(ObserverId, TargetId);

        SetupStreetcodeRepositorySequence(
            CreateStreetcode(ObserverId, ObserverTitle, ObserverUrl),
            CreateStreetcode(TargetId, TargetTitle, TargetUrl));

        _repositoryWrapperMock
            .Setup(wrapper => wrapper.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(0);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsFailed.Should().BeTrue();

        result.Errors.Should()
            .ContainSingle(error =>
                error.Message == ErrorMessages.FailedToCreateRelation);

        _relatedFigureRepositoryMock.Verify(
            repository => repository.Create(
                It.Is<RelatedFigureEntity>(
                    relation => relation.ObserverId == ObserverId &&
                                relation.TargetId == TargetId)),
            Times.Once);

        _repositoryWrapperMock.Verify(
            wrapper => wrapper.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Once);

        _loggerMock.Verify(
            logger => logger.LogError(
                command,
                ErrorMessages.FailedToCreateRelation),
            Times.Once);
    }

    private static StreetcodeContent CreateStreetcode(
        int id,
        string title,
        string transliterationUrl)
    {
        return new StreetcodeContent
        {
            Id = id,
            Title = title,
            TransliterationUrl = transliterationUrl,
        };
    }
    private void SetupStreetcodeRepositorySequence(
        params StreetcodeContent?[] streetcodes)
    {
        var sequence = _streetcodeRepositoryMock
            .SetupSequence(repository => repository.GetFirstOrDefaultAsync(
                It.IsAny<Expression<Func<StreetcodeContent, bool>>>(),
                It.IsAny<Func<IQueryable<StreetcodeContent>,
                    IIncludableQueryable<StreetcodeContent, object>>?>(),
                It.IsAny<CancellationToken>()));

        foreach (var streetcode in streetcodes)
        {
            sequence.ReturnsAsync(streetcode);
        }
    }
}
