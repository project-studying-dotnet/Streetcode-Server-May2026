using System.Linq.Expressions;
using AutoMapper;
using FluentAssertions;
using Moq;
using Streetcode.BLL.DTO.Streetcode.TextContent.Fact;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.Mapping.Streetcode.TextContent;
using Streetcode.BLL.MediatR.Streetcode.Fact.Update;
using Streetcode.DAL.Repositories.Interfaces.Base;
using Streetcode.DAL.Repositories.Interfaces.Streetcode.TextContent;
using Xunit;
using Entity = Streetcode.DAL.Entities.Streetcode.TextContent.Fact;

namespace Streetcode.XUnitTest.BLL.MediatR.Fact.Update;

public class UpdateFactHandlerTests
{
    private readonly Mock<IRepositoryWrapper> _repositoryWrapperMock;
    private readonly Mock<IFactRepository> _factRepositoryMock;
    private readonly Mock<ILoggerService> _loggerMock;
    private readonly IMapper _mapper;
    private readonly UpdateFactHandler _handler;

    public UpdateFactHandlerTests()
    {
        _repositoryWrapperMock = new Mock<IRepositoryWrapper>();
        _factRepositoryMock = new Mock<IFactRepository>();
        _loggerMock = new Mock<ILoggerService>();

        _mapper = new MapperConfiguration(cfg => cfg.AddProfile<FactProfile>()).CreateMapper();

        _repositoryWrapperMock
            .Setup(wrapper => wrapper.FactRepository)
            .Returns(_factRepositoryMock.Object);

        _handler = new UpdateFactHandler(
            _repositoryWrapperMock.Object,
            _mapper,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnFail_WhenFactNotFound()
    {
        var command = new UpdateFactCommand(new FactDto
        {
            Id = 1,
            Title = "Test Title",
            FactContent = "Test Content",
            StreetcodeId = 1,
        });

        _factRepositoryMock
            .Setup(repo => repo.GetFirstOrDefaultAsync(
                It.IsAny<Expression<Func<Entity, bool>>>(),
                null,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((Entity?)null);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsFailed.Should().BeTrue();
        result.Errors[0].Message.Should().Be("Cannot find fact");

        _loggerMock.Verify(
            logger => logger.LogError(command, "Cannot find fact"),
            Times.Once);

        _factRepositoryMock.Verify(repo => repo.Update(It.IsAny<Entity>()), Times.Never);
        _repositoryWrapperMock.Verify(
            wrapper => wrapper.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldReturnOk_WhenFactUpdatedSuccessfully()
    {
        var existingFact = new Entity
        {
            Id = 1,
            Title = "Old Title",
            FactContent = "Old Content",
            Index = 0,
            ImageId = 1,
            StreetcodeId = 1,
        };

        var command = new UpdateFactCommand(new FactDto
        {
            Id = 1,
            Title = "New Title",
            FactContent = "New Content",
            Index = 2,
            ImageId = 2,
            StreetcodeId = 1,
        });

        _factRepositoryMock
            .Setup(repo => repo.GetFirstOrDefaultAsync(
                It.IsAny<Expression<Func<Entity, bool>>>(),
                null,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingFact);

        _repositoryWrapperMock
            .Setup(wrapper => wrapper.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        existingFact.Title.Should().Be(command.Fact.Title);
        existingFact.FactContent.Should().Be(command.Fact.FactContent);
        existingFact.Index.Should().Be(command.Fact.Index);
        existingFact.ImageId.Should().Be(command.Fact.ImageId);

        _factRepositoryMock.Verify(repo => repo.Update(existingFact), Times.Once);
        _repositoryWrapperMock.Verify(
            wrapper => wrapper.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Once);
        _loggerMock.Verify(logger => logger.LogError(It.IsAny<object>(), It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldReturnFail_WhenSaveChangesReturnsZero()
    {
        var existingFact = new Entity
        {
            Id = 1,
            Title = "Old Title",
            FactContent = "Old Content",
            StreetcodeId = 1,
        };

        var command = new UpdateFactCommand(new FactDto
        {
            Id = 1,
            Title = "New Title",
            FactContent = "New Content",
            StreetcodeId = 1,
        });

        _factRepositoryMock
            .Setup(repo => repo.GetFirstOrDefaultAsync(
                It.IsAny<Expression<Func<Entity, bool>>>(),
                null,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingFact);

        _repositoryWrapperMock
            .Setup(wrapper => wrapper.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(0);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsFailed.Should().BeTrue();
        result.Errors[0].Message.Should().Be("Cannot save fact to database");

        _loggerMock.Verify(
            logger => logger.LogError(command, "Cannot save fact to database"),
            Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldReturnFail_WhenRepositoryThrowsException()
    {
        var command = new UpdateFactCommand(new FactDto
        {
            Id = 1,
            Title = "Test Title",
            FactContent = "Test Content",
            StreetcodeId = 1,
        });

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
