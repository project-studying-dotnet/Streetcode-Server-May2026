using FluentAssertions;
using FluentResults;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MediatR;
using Moq;
using Streetcode.BLL.DTO.News;
using Streetcode.BLL.MediatR.Newss.SortedByDateTime;
using Streetcode.WebApi.Controllers;
using Xunit;

namespace Streetcode.XUnitTest.Controllers
{
    public class NewsControllerGetSortedByDateTimeTests
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly NewsController _controller;

        public NewsControllerGetSortedByDateTimeTests()
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
        public async Task GetSortedByDateTime_ReturnsOkResult_WithSortedNewsList_WhenNewsExist()
        {
            var expectedSortedNews = new List<NewsDTO>
            {
                new NewsDTO { Id = 3, Title = "New News" }
            };

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<SortedByDateTimeQuery>(), default))
                .ReturnsAsync(Result.Ok<List<NewsDTO>>(expectedSortedNews));

            var result = await _controller.GetSortedByDateTime();

            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
            okResult.Value.Should().BeEquivalentTo(expectedSortedNews);

            _mediatorMock.Verify(m => m.Send(It.IsAny<SortedByDateTimeQuery>(), default), Times.Once);
        }

        [Fact]
        public async Task GetSortedByDateTime_ReturnsBadRequestOrNotFound_WhenRequestFails()
        {
            _mediatorMock
                .Setup(m => m.Send(It.IsAny<SortedByDateTimeQuery>(), default))
                .ReturnsAsync(Result.Fail<List<NewsDTO>>("Failed to fetch sorted news"));

            var result = await _controller.GetSortedByDateTime();

            result.Should().NotBeOfType<OkObjectResult>();

            _mediatorMock.Verify(m => m.Send(It.IsAny<SortedByDateTimeQuery>(), default), Times.Once);
        }
    }
}