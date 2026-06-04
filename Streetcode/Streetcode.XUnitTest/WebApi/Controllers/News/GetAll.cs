using FluentAssertions;
using FluentResults;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MediatR;
using Moq;
using Streetcode.BLL.DTO.News;
using Streetcode.BLL.MediatR.Newss.GetAll;
using Streetcode.WebApi.Controllers;
using Xunit;

namespace Streetcode.XUnitTest.Controllers
{
    public class NewsControllerGetAllTests
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly NewsController _controller;

        public NewsControllerGetAllTests()
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
        public async Task GetAll_ReturnsOkResult_WithListOfNews_WhenNewsExist()
        {
            var expectedNews = new List<NewsDTO> { new NewsDTO { Id = 1 } };
            _mediatorMock
                .Setup(m => m.Send(It.IsAny<GetAllNewsQuery>(), default))
                .ReturnsAsync(Result.Ok<IEnumerable<NewsDTO>>(expectedNews));

            var result = await _controller.GetAll();

            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
            okResult.Value.Should().BeEquivalentTo(expectedNews);
        }

        [Fact]
        public async Task GetAll_ReturnsBadRequestOrNotFound_WhenRequestFails()
        {
            _mediatorMock
                .Setup(m => m.Send(It.IsAny<GetAllNewsQuery>(), default))
                .ReturnsAsync(Result.Fail("Failed to get news"));

            var result = await _controller.GetAll();

            result.Should().NotBeOfType<OkObjectResult>();
        }
    }
}