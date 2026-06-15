using AutoMapper;
using FluentAssertions;
using Moq;
using Streetcode.BLL.DTO.Media.ArtSlidesTemplates;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.Mapping.Media.Images;
using Streetcode.BLL.MediatR.Media.ArtSlideTeamplates.GetAll;
using Streetcode.BLL.Resources;
using Streetcode.DAL.Entities.Media.Images;
using Streetcode.DAL.Repositories.Interfaces.Base;
using Streetcode.DAL.Repositories.Interfaces.Media.Images;
using Xunit;

namespace Streetcode.XUnitTest.BLL.MediatR.Media.ArtSlideTemplate.GetAll;

public class GetAllArtSlideTemplatesHandlerTests
{
    private readonly Mock<IRepositoryWrapper> _repoWrapperMock;
    private readonly Mock<IStreetcodeArtSlideTemplateRepository> _templateRepoMock;
    private readonly IMapper _mapper;
    private readonly Mock<ILoggerService> _loggerMock;
    private readonly GetAllArtSlideTemplatesHandler _handler;

    public GetAllArtSlideTemplatesHandlerTests()
    {
        var config = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<ArtSlideTeamplatesProfile>();
        });

        _mapper = config.CreateMapper();

        _repoWrapperMock = new Mock<IRepositoryWrapper>();
        _templateRepoMock = new Mock<IStreetcodeArtSlideTemplateRepository>();
        _loggerMock = new Mock<ILoggerService>();

        _repoWrapperMock.Setup(r => r.StreetcodeArtSlideTemplateRepository).Returns(_templateRepoMock.Object);
        _handler = new GetAllArtSlideTemplatesHandler(_repoWrapperMock.Object, _mapper, _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnTemplates_WhenTemplatesExist()
    {
        // Arrange
        var templates = new List<StreetcodeArtSlideTemplate> { new() { Id = 1 } };
        _templateRepoMock.Setup(r => r.GetAllAsync(null, null)).ReturnsAsync(templates);

        // Act
        var result = await _handler.Handle(new GetAllArtSlideTemplatesQuery(), CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(1);
        _loggerMock.Verify(l => l.LogError(It.IsAny<object>(), It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldReturnFail_WhenTemplatesAreEmpty()
    {
        // Arrange
        _templateRepoMock.Setup(r => r.GetAllAsync(null, null)).ReturnsAsync(new List<StreetcodeArtSlideTemplate>());

        // Act
        var result = await _handler.Handle(new GetAllArtSlideTemplatesQuery(), CancellationToken.None);

        // Assert
        result.IsFailed.Should().BeTrue();
        string expectedMessage = ErrorMessages.CannotFindAnyArtSlideTemplates ?? "Cannot find any art slide templates.";

        result.Errors.Should().ContainSingle(e => e.Message == expectedMessage);

        _loggerMock.Verify(l => l.LogError(It.IsAny<object>(), expectedMessage), Times.Once);
    }
}