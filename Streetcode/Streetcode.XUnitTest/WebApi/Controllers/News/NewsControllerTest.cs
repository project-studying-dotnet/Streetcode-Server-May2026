using FluentAssertions;
using FluentResults;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MediatR;
using Moq;
using Streetcode.BLL.DTO.News;
using Streetcode.BLL.MediatR.Newss.GetNewsAndLinksByUrl;
using Streetcode.WebApi.Controllers;
using Xunit;

namespace Streetcode.XUnitTest.Controllers
{
    public class NewsControllerTests
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly NewsController _controller;

        public NewsControllerTests()
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
        public async Task GetNewsAndLinksByUrl_ShouldReturnOk_WhenUrlIsValidAndNewsExists()
        {
            var testUrl = "news:string";

            var expectedResponse = new NewsDTOWithURLs
            {
                News = new NewsDTO { Id = 9, URL = testUrl, Title = "Тестова новина" },
                PrevNewsUrl = "news:prev",
                NextNewsUrl = "news:next",
                RandomNews = new RandomNewsDTO { RandomNewsUrl = "news:id3", Title = "Випадкова новина" }
            };

            _mediatorMock
                .Setup(m => m.Send(It.Is<GetNewsAndLinksByUrlQuery>(q => q.url == testUrl), default))
                .ReturnsAsync(Result.Ok(expectedResponse));

            var result = await _controller.GetNewsAndLinksByUrl(testUrl);

            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
            okResult.Value.Should().BeEquivalentTo(expectedResponse);

            _mediatorMock.Verify(m => m.Send(It.IsAny<GetNewsAndLinksByUrlQuery>(), default), Times.Once);
        }

        [Fact]
        public async Task GetNewsAndLinksByUrl_ShouldReturnBadRequest_WhenModelStateIsInvalid()
        {
            var invalidUrl = "";
            _controller.ModelState.AddModelError("url", "Url is required");

            IActionResult result;
            if (!_controller.ModelState.IsValid)
            {
                result = _controller.BadRequest(_controller.ModelState);
            }
            else
            {
                result = await _controller.GetNewsAndLinksByUrl(invalidUrl);
            }

            result.Should().BeOfType<BadRequestObjectResult>();
            _mediatorMock.Verify(m => m.Send(It.IsAny<GetNewsAndLinksByUrlQuery>(), default), Times.Never);
        }
    }
}