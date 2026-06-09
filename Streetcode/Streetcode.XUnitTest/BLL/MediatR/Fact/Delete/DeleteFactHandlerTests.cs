using System.Linq.Expressions;
using AutoMapper;
using FluentAssertions;
using Moq;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.Mapping.Streetcode.TextContent;
using Streetcode.BLL.MediatR.Streetcode.Fact.Delete;
using Streetcode.DAL.Repositories.Interfaces.Base;
using Streetcode.DAL.Repositories.Interfaces.Streetcode.TextContent;
using Xunit;
using Entity = Streetcode.DAL.Entities.Streetcode.TextContent.Fact;

namespace Streetcode.XUnitTest.BLL.MediatR.Fact.Delete;

public class DeleteFactHandlerTests
{
    private readonly Mock<IRepositoryWrapper> _repositoryWrapperMock;
    private readonly Mock<IFactRepository> _factRepositoryMock;
    private readonly Mock<ILoggerService> _loggerMock;
    private readonly IMapper _mapper;
    private readonly DeleteFactHandler _handler;

    public DeleteFactHandlerTests()
    {
        _repositoryWrapperMock = new Mock<IRepositoryWrapper>();
        _factRepositoryMock = new Mock<IFactRepository>();
        _loggerMock = new Mock<ILoggerService>();

        _mapper = new MapperConfiguration(cfg => cfg.AddProfile<FactProfile>()).CreateMapper();

        _repositoryWrapperMock
            .Setup(wrapper => wrapper.FactRepository)
            .Returns(_factRepositoryMock.Object);

        _handler = new DeleteFactHandler(
            _repositoryWrapperMock.Object,
            _mapper,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnFail_WhenFactNotFound()
    {
        var command = new DeleteFactCommand(1);

        _factRepositoryMock
            .Setup(repo => repo.GetFirstOrDefaultAsync(
                It.IsAny<Expression<Func<Entity, bool>>>(),
                null,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((Entity?)null);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsFailed.Should().BeTrue();
        result.Errors[0].Message.Should().Be("Cannot find fact with id: 1");

        _loggerMock.Verify(
            logger => logger.LogError(command, "Cannot find fact with id: 1"),
            Times.Once);

        _factRepositoryMock.Verify(repo => repo.Delete(It.IsAny<Entity>()), Times.Never);
        _repositoryWrapperMock.Verify(
            wrapper => wrapper.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldReturnOk_WhenFactDeletedSuccessfully()
    {
        var command = new DeleteFactCommand(1);
        var fact = new Entity
        {
            Id = 1,
            Title = "Test Title",
            FactContent = "Test Content",
            Index = 1,
            ImageId = 1,
            StreetcodeId = 1,
        };

        _factRepositoryMock
            .Setup(repo => repo.GetFirstOrDefaultAsync(
                It.IsAny<Expression<Func<Entity, bool>>>(),
                null,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(fact);

        _repositoryWrapperMock
            .Setup(wrapper => wrapper.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Id.Should().Be(fact.Id);
        result.Value.Title.Should().Be(fact.Title);
        result.Value.FactContent.Should().Be(fact.FactContent);

        _factRepositoryMock.Verify(repo => repo.Delete(fact), Times.Once);
        _repositoryWrapperMock.Verify(
            wrapper => wrapper.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Once);
        _loggerMock.Verify(logger => logger.LogError(It.IsAny<object>(), It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldReturnFail_WhenSaveChangesReturnsZero()
    {
        var command = new DeleteFactCommand(1);
        var fact = new Entity
        {
            Id = 1,
            Title = "Test Title",
            FactContent = "Test Content",
            StreetcodeId = 1,
        };

        _factRepositoryMock
            .Setup(repo => repo.GetFirstOrDefaultAsync(
                It.IsAny<Expression<Func<Entity, bool>>>(),
                null,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(fact);

        _repositoryWrapperMock
            .Setup(wrapper => wrapper.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(0);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsFailed.Should().BeTrue();
        result.Errors[0].Message.Should().Be("Failed to delete fact");

        _factRepositoryMock.Verify(repo => repo.Delete(fact), Times.Once);
        _loggerMock.Verify(
            logger => logger.LogError(command, "Failed to delete fact"),
            Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldReturnFail_WhenRepositoryThrowsException()
    {
        var command = new DeleteFactCommand(1);

        _factRepositoryMock
            .Setup(repo => repo.GetFirstOrDefaultAsync(
                It.IsAny<Expression<Func<Entity, bool>>>(),
                null,
                It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Test exception"));

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsFailed.Should().BeTrue();
        result.Errors[0].Message.Should().Be("Test exception");

        _loggerMock.Verify(
            logger => logger.LogError(command, "Test exception"),
            Times.Once);
    }
}
