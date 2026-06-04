using FluentAssertions;
using FluentResults;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MediatR;
using Moq;
using Streetcode.BLL.DTO.News;
using Streetcode.BLL.MediatR.Newss.Create;
using Streetcode.WebApi.Controllers;
using Xunit;

namespace Streetcode.XUnitTest.Controllers
{
    public class CreateTests
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly NewsController _controller;

        public CreateTests()
        {
            _mediatorMock = new Mock<IMediator>();
            _controller = new NewsController();

            var serviceProviderMock = new Mock<IServiceProvider>();
            serviceProviderMock
                .Setup(s => s.GetService(typeof(IMediator)))
                .Returns(_mediatorMock.Object);

            var httpContextMock = new Mock<HttpContext>();
            httpContextMock
                .Setup(c => c.RequestServices)
                .Returns(serviceProviderMock.Object);

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContextMock.Object
            };
        }

        [Fact]
        public async Task Create_ReturnsOkResult_WithCreatedNews_WhenSuccessful()
        {
            var newsDto = new NewsDTO { Title = "New News" };
            var expectedResponse = new NewsDTO { Id = 1, Title = "New News" };

            _mediatorMock
                .Setup(m => m.Send(It.Is<CreateNewsCommand>(c => c.newNews == newsDto), default))
                .ReturnsAsync(Result.Ok(expectedResponse));

            var result = await _controller.Create(newsDto);

            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
            okResult.Value.Should().BeEquivalentTo(expectedResponse);

            _mediatorMock.Verify(m => m.Send(It.IsAny<CreateNewsCommand>(), default), Times.Once);
        }

        [Fact]
        public async Task Create_ReturnsBadRequestOrNotFound_WhenRequestFails()
        {
            var newsDto = new NewsDTO { Title = "New News" };

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<CreateNewsCommand>(), default))
                .ReturnsAsync(Result.Fail<NewsDTO>("Failed to create news"));

            var result = await _controller.Create(newsDto);

            result.Should().NotBeOfType<OkObjectResult>();

            _mediatorMock.Verify(m => m.Send(It.IsAny<CreateNewsCommand>(), default), Times.Once);
        }
    }
}