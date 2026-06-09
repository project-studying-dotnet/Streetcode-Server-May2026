using FluentAssertions;
using FluentResults;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MediatR;
using Moq;
using Streetcode.BLL.DTO.News;
using Streetcode.BLL.MediatR.Newss.Update;
using Streetcode.WebApi.Controllers.News;
using Xunit;

namespace Streetcode.XUnitTest.Controllers
{
    public class Update
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly NewsController _controller;

        public Update()
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
        public async Task Update_ReturnsOkResult_WithUpdatedNews_WhenSuccessful()
        {
            int testId = 1;
            var newsDto = new NewsDTO { Id = 0, Title = "Updated News" };
            var expectedResponse = new NewsDTO { Id = testId, Title = "Updated News" };

            _mediatorMock
                .Setup(m => m.Send(It.Is<UpdateNewsCommand>(c => c.news.Id == testId), default))
                .ReturnsAsync(Result.Ok(expectedResponse));

            var result = await _controller.Update(testId, newsDto);

            newsDto.Id.Should().Be(testId);
            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
            okResult.Value.Should().BeEquivalentTo(expectedResponse);

            _mediatorMock.Verify(m => m.Send(It.IsAny<UpdateNewsCommand>(), default), Times.Once);
        }

        [Fact]
        public async Task Update_ReturnsBadRequestOrNotFound_WhenRequestFails()
        {
            int testId = 1;
            var newsDto = new NewsDTO { Id = 0, Title = "Updated News" };

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<UpdateNewsCommand>(), default))
                .ReturnsAsync(Result.Fail<NewsDTO>("Failed to update news"));

            var result = await _controller.Update(testId, newsDto);

            result.Should().NotBeOfType<OkObjectResult>();

            _mediatorMock.Verify(m => m.Send(It.IsAny<UpdateNewsCommand>(), default), Times.Once);
        }
    }
}