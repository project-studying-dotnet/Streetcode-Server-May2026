using AutoMapper;
using FluentAssertions;
using Moq;
using Streetcode.BLL.DTO.Streetcode.TextContent.Fact;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.Mapping.Streetcode.TextContent;
using Streetcode.BLL.MediatR.Streetcode.Fact.Create;
using Streetcode.DAL.Repositories.Interfaces.Base;
using Streetcode.DAL.Repositories.Interfaces.Streetcode.TextContent;
using Xunit;
using Entity = Streetcode.DAL.Entities.Streetcode.TextContent.Fact;

namespace Streetcode.XUnitTest.BLL.MediatR.Fact.Create;

public class CreateFactHandlerTests
{
    private readonly Mock<IRepositoryWrapper> _repositoryWrapperMock;
    private readonly Mock<IFactRepository> _factRepositoryMock;
    private readonly Mock<ILoggerService> _loggerMock;
    private readonly IMapper _mapper;
    private readonly CreateFactHandler _handler;

    public CreateFactHandlerTests()
    {
        _repositoryWrapperMock = new Mock<IRepositoryWrapper>();
        _factRepositoryMock = new Mock<IFactRepository>();
        _loggerMock = new Mock<ILoggerService>();

        _mapper = new MapperConfiguration(cfg => cfg.AddProfile<FactProfile>()).CreateMapper();

        _repositoryWrapperMock
            .Setup(wrapper => wrapper.FactRepository)
            .Returns(_factRepositoryMock.Object);

        _handler = new CreateFactHandler(
            _repositoryWrapperMock.Object,
            _mapper,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnOk_WhenFactCreatedSuccessfully()
    {
        var factDto = new FactDto
        {
            Title = "Test Title",
            FactContent = "Test Content",
            Index = 1,
            ImageId = 1,
            StreetcodeId = 1,
        };
        var command = new CreateFactCommand(factDto);

        _factRepositoryMock
            .Setup(repo => repo.CreateAsync(It.IsAny<Entity>()))
            .ReturnsAsync((Entity fact) => fact);

        _repositoryWrapperMock
            .Setup(wrapper => wrapper.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Title.Should().Be(factDto.Title);
        result.Value.FactContent.Should().Be(factDto.FactContent);
        result.Value.Index.Should().Be(factDto.Index);
        result.Value.ImageId.Should().Be(factDto.ImageId);
        result.Value.StreetcodeId.Should().Be(factDto.StreetcodeId);

        _factRepositoryMock.Verify(repo => repo.CreateAsync(It.IsAny<Entity>()), Times.Once);
        _repositoryWrapperMock.Verify(wrapper => wrapper.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        _loggerMock.Verify(logger => logger.LogError(It.IsAny<object>(), It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldReturnFail_WhenSaveChangesReturnsZero()
    {
        var command = new CreateFactCommand(new FactDto
        {
            Title = "Test Title",
            FactContent = "Test Content",
            StreetcodeId = 1,
        });

        _factRepositoryMock
            .Setup(repo => repo.CreateAsync(It.IsAny<Entity>()))
            .ReturnsAsync((Entity fact) => fact);

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
        var command = new CreateFactCommand(new FactDto
        {
            Title = "Test Title",
            FactContent = "Test Content",
            StreetcodeId = 1,
        });

        _factRepositoryMock
            .Setup(repo => repo.CreateAsync(It.IsAny<Entity>()))
            .ThrowsAsync(new Exception("Test exception"));

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsFailed.Should().BeTrue();
        result.Errors[0].Message.Should().Be("Test exception");

        _loggerMock.Verify(
            logger => logger.LogError(command, "Test exception"),
            Times.Once);
    }
}
