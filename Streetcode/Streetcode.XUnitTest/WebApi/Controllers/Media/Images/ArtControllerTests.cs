using System.Reflection;
using FluentAssertions;
using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Streetcode.BLL.DTO.Media.Art;
using Streetcode.BLL.MediatR.Media.Art.Create;
using Streetcode.BLL.MediatR.Media.Art.GetAll;
using Streetcode.BLL.MediatR.Media.Art.GetById;
using Streetcode.WebApi.Controllers;
using Streetcode.WebApi.Controllers.Media.Images;
using Xunit;

namespace Streetcode.XUnitTest.WebApi.Controllers.Media.Images;

public class ArtControllerTests
{
    private readonly Mock<IMediator> _mediatorMock;
    private readonly ArtController _controller;

    public ArtControllerTests()
    {
        _mediatorMock = new Mock<IMediator>();
        _controller = new ArtController();

        var type = typeof(BaseApiController);

        // ❗ 1. ИЩЕМ FIELD (реальный источник Mediator)
        var field =
            type.GetField("_mediator", BindingFlags.Instance | BindingFlags.NonPublic)
            ?? type.GetFields(BindingFlags.Instance | BindingFlags.NonPublic)
                .FirstOrDefault(f => f.FieldType == typeof(IMediator));

        if (field == null)
            throw new Exception("Cannot find IMediator field in BaseApiController");

        field.SetValue(_controller, _mediatorMock.Object);
    }

    [Fact]
    public async Task GetAll_ShouldReturnOk_WhenResultIsSuccess()
    {
        var expected = Result.Ok<IEnumerable<ArtDTO>>(new List<ArtDTO>());

        _mediatorMock
            .Setup(m => m.Send(
                It.IsAny<GetAllArtsQuery>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(expected);

        var result = await _controller.GetAll();

        result.Should().BeOfType<OkObjectResult>();

        _mediatorMock.Verify(
            m => m.Send(It.IsAny<GetAllArtsQuery>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task GetById_ShouldReturnOk_WhenArtExists()
    {
        const int id = 1;

        var expected = Result.Ok(new ArtDTO
        {
            Id = id
        });

        _mediatorMock
            .Setup(m => m.Send(
                It.IsAny<GetArtByIdQuery>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(expected);

        var result = await _controller.GetById(id);

        result.Should().BeOfType<OkObjectResult>();

        _mediatorMock.Verify(
            m => m.Send(It.IsAny<GetArtByIdQuery>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Create_ShouldReturnOk_WhenDataIsValid()
    {
        var dto = new ArtCreateDto();

        var expected = Result.Ok(new ArtDTO());

        _mediatorMock
            .Setup(m => m.Send(
                It.IsAny<CreateArtCommand>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(expected);

        var result = await _controller.Create(dto);

        result.Should().BeOfType<OkObjectResult>();

        _mediatorMock.Verify(
            m => m.Send(It.IsAny<CreateArtCommand>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }
}