using FluentAssertions;
using FluentResults;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MediatR;
using Moq;
using Streetcode.BLL.DTO.News;
using Streetcode.BLL.MediatR.Newss.GetByUrl;
using Streetcode.WebApi.Controllers;
using Xunit;

namespace Streetcode.XUnitTest.Controllers
{
    public class GetByUrl
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly NewsController _controller;

        public GetByUrl()
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
        public async Task GetByUrl_ReturnsOkResult_WithNews_WhenNewsExists()
        {
            var testUrl = "news-url-string";
            var expectedNews = new NewsDTO
            {
                Id = 1,
                Title = "Test News by URL",
                URL = testUrl
            };

            _mediatorMock
                .Setup(m => m.Send(It.Is<GetNewsByUrlQuery>(q => q.url == testUrl), default))
                .ReturnsAsync(Result.Ok(expectedNews));

            var result = await _controller.GetByUrl(testUrl);

            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
            okResult.Value.Should().BeEquivalentTo(expectedNews);

            _mediatorMock.Verify(m => m.Send(It.IsAny<GetNewsByUrlQuery>(), default), Times.Once);
        }

        [Fact]
        public async Task GetByUrl_ReturnsBadRequestOrNotFound_WhenNewsDoesNotExist()
        {
            var testUrl = "non-existing-url";

            _mediatorMock
                .Setup(m => m.Send(It.Is<GetNewsByUrlQuery>(q => q.url == testUrl), default))
                .ReturnsAsync(Result.Fail<NewsDTO>("News with this URL was not found"));

            var result = await _controller.GetByUrl(testUrl);

            result.Should().NotBeOfType<OkObjectResult>();

            _mediatorMock.Verify(m => m.Send(It.IsAny<GetNewsByUrlQuery>(), default), Times.Once);
        }
    }
}