using System.Reflection;
using FluentAssertions;
using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Streetcode.BLL.DTO.Media.ArtSlidesTemplates;
using Streetcode.BLL.MediatR.Media.ArtSlideTeamplates.GetAll;
using Streetcode.WebApi.Controllers;
using Streetcode.WebApi.Controllers.Media;
using Xunit;

namespace Streetcode.XUnitTest.WebApi.Controllers.Media;

public class ArtSlideTeamplatesControllerTests
{
    private readonly Mock<IMediator> _mediatorMock;
    private readonly ArtSlideTeamplatesController _controller;

    public ArtSlideTeamplatesControllerTests()
    {
        _mediatorMock = new Mock<IMediator>();
        _controller = new ArtSlideTeamplatesController();

        var type = typeof(BaseApiController);

        var field =
            type.GetField("_mediator", BindingFlags.Instance | BindingFlags.NonPublic)
            ?? type.GetFields(BindingFlags.Instance | BindingFlags.NonPublic)
                .FirstOrDefault(f => f.FieldType == typeof(IMediator));

        if (field == null)
           {
            throw new Exception("Mediator field not found in BaseApiController");
           }

        field.SetValue(_controller, _mediatorMock.Object);
    }

    [Fact]
    public async Task GetAll_ShouldReturnOk_WhenDataExists()
    {
        // Arrange
        _mediatorMock
            .Setup(m => m.Send(
                It.IsAny<GetAllArtSlideTemplatesQuery>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(
                Result.Ok<IEnumerable<StreetcodeArtSlideTemplateDto>>(
                    new List<StreetcodeArtSlideTemplateDto>()));

        // Act
        var result = await _controller.GetAll();

        // Assert
        result.Should().BeOfType<OkObjectResult>();

        _mediatorMock.Verify(
            m => m.Send(
                It.IsAny<GetAllArtSlideTemplatesQuery>(),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }
}