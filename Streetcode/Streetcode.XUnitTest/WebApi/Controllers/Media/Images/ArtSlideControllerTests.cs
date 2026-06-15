using System.Reflection;
using FluentAssertions;
using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Streetcode.BLL.DTO.Media.ArtSlides;
using Streetcode.BLL.MediatR.Media.ArtSlide.Create;
using Streetcode.BLL.MediatR.Media.ArtSlide.Delete;
using Streetcode.BLL.MediatR.Media.ArtSlide.GetAllByStreetcodeId;
using Streetcode.BLL.MediatR.Media.ArtSlide.Update;
using Streetcode.WebApi.Controllers;
using Streetcode.WebApi.Controllers.Media;
using Xunit;

namespace Streetcode.XUnitTest.WebApi.Controllers.Media;

public class ArtSlideControllerTests
{
    private readonly Mock<IMediator> _mediatorMock;
    private readonly ArtSlideController _controller;

    public ArtSlideControllerTests()
    {
        _mediatorMock = new Mock<IMediator>();
        _controller = new ArtSlideController();

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
    public async Task GetAllByStreetcodeId_ShouldReturnOk()
    {
        const int streetcodeId = 1;

        _mediatorMock
            .Setup(m => m.Send(
                It.IsAny<GetAllArtSlidesByStreetcodeIdQuery>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Ok<IEnumerable<StreetcodeArtSlideDto>>(
                new List<StreetcodeArtSlideDto>()));

        var result = await _controller.GetAllByStreetcodeId(streetcodeId);

        result.Should().BeOfType<OkObjectResult>();

        _mediatorMock.Verify(
            m => m.Send(It.IsAny<GetAllArtSlidesByStreetcodeIdQuery>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Create_ShouldCallMediator()
    {
        var dto = new CreateStreetcodeArtSlideDto
        {
            StreetcodeId = 1,
            TemplateId = 1,
            Index = 0
        };

        _mediatorMock
            .Setup(m => m.Send(
                It.IsAny<CreateArtSlideCommand>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Ok(new StreetcodeArtSlideDto()));

        var result = await _controller.Create(dto);

        result.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public async Task Update_ShouldCallMediator()
    {
        var dto = new UpdateArtSlideDto
        {
            Id = 1,
            StreetcodeId = 1,
            TemplateId = 1,
            Index = 0
        };

        _mediatorMock
            .Setup(m => m.Send(
                It.IsAny<UpdateArtSlideCommand>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Ok(Unit.Value));

        var result = await _controller.Update(dto);

        result.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public async Task Delete_ShouldCallMediator()
    {
        const int id = 1;

        _mediatorMock
            .Setup(m => m.Send(
                It.IsAny<DeleteArtSlideCommand>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Ok());

        var result = await _controller.Delete(id);

        result.Should().BeOfType<OkObjectResult>();
    }
}