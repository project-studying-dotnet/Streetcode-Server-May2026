using FluentAssertions;
using FluentResults;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MediatR;
using Moq;
using Streetcode.BLL.DTO.News;
using Streetcode.BLL.MediatR.Newss.GetNewsAndLinksByUrl;
using Streetcode.WebApi.Controllers;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Streetcode.XUnitTest.Controllers
{
    public class NewsControllerGetNewsAndLinksByUrlTests
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly NewsController _controller;

        public NewsControllerGetNewsAndLinksByUrlTests()
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
        public async Task GetNewsAndLinksByUrl_ReturnsOkResult_WhenNewsExists()
        {
            var testUrl = "news:string";
            var expectedResponse = new NewsDTOWithURLs
            {
                News = new NewsDTO { Id = 9, URL = testUrl, Title = "Тестова новина" }
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
        public async Task GetNewsAndLinksByUrl_ReturnsBadRequestOrNotFound_WhenRequestFails()
        {
            var testUrl = "news:string";

            _mediatorMock
                .Setup(m => m.Send(It.Is<GetNewsAndLinksByUrlQuery>(q => q.url == testUrl), default))
                .ReturnsAsync(Result.Fail<NewsDTOWithURLs>("News not found"));

            var result = await _controller.GetNewsAndLinksByUrl(testUrl);

            result.Should().NotBeOfType<OkObjectResult>();

            _mediatorMock.Verify(m => m.Send(It.IsAny<GetNewsAndLinksByUrlQuery>(), default), Times.Once);
        }
    }
}