using AutoMapper;
using FluentAssertions;
using Moq;
using Streetcode.BLL.DTO.AdditionalContent;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.MediatR.AdditionalContent.Tag.Create;
using Streetcode.DAL.Repositories.Interfaces.Base;
using Xunit;

using Streetcode.BLL.DTO.AdditionalContent.Tag;
using TagEntity = global::Streetcode.DAL.Entities.AdditionalContent.Tag;

namespace Streetcode.XUnitTest.BLL.MediatR.AdditionalContent.Tag.Create;

public class CreateTagHandlerTests
{
    private const string TagTitle = "Test tag";
    private const string SaveChangesErrorMessage = "Database error";

    private readonly Mock<IRepositoryWrapper> _repositoryWrapperMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<ILoggerService> _loggerMock;
    private readonly CreateTagHandler _handler;

    public CreateTagHandlerTests()
    {
        _repositoryWrapperMock = new Mock<IRepositoryWrapper>();
        _mapperMock = new Mock<IMapper>();
        _loggerMock = new Mock<ILoggerService>();

        _handler = new CreateTagHandler(
            _repositoryWrapperMock.Object,
            _mapperMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnOk_WhenTagCreated()
    {
        var request = CreateRequest();

        var createdTag = new TagEntity
        {
            Id = 1,
            Title = TagTitle,
        };

        var expectedDto = new TagDTO
        {
            Id = 1,
            Title = TagTitle,
        };

        _repositoryWrapperMock
            .Setup(repository => repository.TagRepository.CreateAsync(
                It.Is<TagEntity>(tag => tag.Title == TagTitle)))
            .ReturnsAsync(createdTag);

        _repositoryWrapperMock
            .Setup(repository => repository.SaveChanges())
            .Returns(1);

        _mapperMock
            .Setup(mapper => mapper.Map<TagDTO>(createdTag))
            .Returns(expectedDto);

        var result = await _handler.Handle(
            request,
            CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEquivalentTo(expectedDto);

        _repositoryWrapperMock.Verify(
            repository => repository.TagRepository.CreateAsync(
                It.Is<TagEntity>(tag => tag.Title == TagTitle)),
            Times.Once);

        _repositoryWrapperMock.Verify(
            repository => repository.SaveChanges(),
            Times.Once);

        _loggerMock.Verify(
            logger => logger.LogError(
                It.IsAny<object>(),
                It.IsAny<string>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldReturnFail_WhenSaveChangesThrowsException()
    {
        var request = CreateRequest();

        var createdTag = new TagEntity
        {
            Id = 1,
            Title = TagTitle,
        };

        _repositoryWrapperMock
            .Setup(repository => repository.TagRepository.CreateAsync(
                It.IsAny<TagEntity>()))
            .ReturnsAsync(createdTag);

        _repositoryWrapperMock
            .Setup(repository => repository.SaveChanges())
            .Throws(new Exception(SaveChangesErrorMessage));

        var result = await _handler.Handle(
            request,
            CancellationToken.None);

        result.IsFailed.Should().BeTrue();

        result.Errors.Should()
            .ContainSingle(error =>
                error.Message.Contains(SaveChangesErrorMessage));

        _repositoryWrapperMock.Verify(
            repository => repository.TagRepository.CreateAsync(
                It.Is<TagEntity>(tag => tag.Title == TagTitle)),
            Times.Once);

        _repositoryWrapperMock.Verify(
            repository => repository.SaveChanges(),
            Times.Once);

        _loggerMock.Verify(
            logger => logger.LogError(
                request,
                It.Is<string>(message =>
                    message.Contains(SaveChangesErrorMessage))),
            Times.Once);

        _mapperMock.Verify(
            mapper => mapper.Map<TagDTO>(It.IsAny<TagEntity>()),
            Times.Never);
    }

    private static CreateTagQuery CreateRequest()
    {
        return new CreateTagQuery(new CreateTagDTO
        {
            Title = TagTitle,
        });
    }
}