using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Streetcode.BLL.DTO.Comments;
using Streetcode.BLL.MediatR.Comments.Create;
using Streetcode.BLL.MediatR.Comments.Update;
using Streetcode.WebApi.Controllers.Comments;
using Xunit;

namespace Streetcode.XUnitTest.WebApi.Controllers.Comments;

public class CommentControllerTests
{
    private const int CommentId = 1;
    private const int StreetcodeId = 2;
    private const string CommentText = "Test comment";

    private readonly Mock<IMediator> _mediatorMock;
    private readonly CommentController _controller;

    public CommentControllerTests()
    {
        _mediatorMock = new Mock<IMediator>();
        _controller = new CommentController
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    RequestServices = new ServiceCollection()
                        .AddSingleton(_mediatorMock.Object)
                        .BuildServiceProvider(),
                },
            },
        };
    }

    [Fact]
    public async Task Create_ShouldReturnOk_WhenResultIsSuccess()
    {
        var createComment = new CreateCommentDto
        {
            Text = CommentText,
            StreetcodeId = StreetcodeId,
        };
        var expectedResult = CreateCommentDto();

        _mediatorMock
            .Setup(x => x.Send(
                It.IsAny<CreateCommentCommand>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Ok(expectedResult));

        var result = await _controller.Create(createComment);

        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(expectedResult, okResult.Value);
    }

    [Fact]
    public async Task Update_ShouldReturnOk_WhenResultIsSuccess()
    {
        var updateComment = new UpdateCommentDto
        {
            Id = CommentId,
            Text = "Updated comment",
            StreetcodeId = StreetcodeId,
        };
        var expectedResult = CreateCommentDto();
        expectedResult.Text = "Updated comment";

        _mediatorMock
            .Setup(x => x.Send(
                It.IsAny<UpdateCommentCommand>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Ok(expectedResult));

        var result = await _controller.Update(updateComment);

        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(expectedResult, okResult.Value);
    }

    private static CommentDto CreateCommentDto()
    {
        return new CommentDto
        {
            Id = CommentId,
            Text = CommentText,
            StreetcodeId = StreetcodeId,
            CreatedAt = DateTime.UtcNow,
        };
    }
}
