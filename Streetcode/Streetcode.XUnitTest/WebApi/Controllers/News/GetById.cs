using FluentAssertions;
using FluentResults;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MediatR;
using Moq;
using Streetcode.BLL.DTO.News;
using Streetcode.BLL.MediatR.Newss.GetById;
using Streetcode.WebApi.Controllers;
using Xunit;

namespace Streetcode.XUnitTest.Controllers
{
    public class NewsControllerGetByIdTests
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly NewsController _controller;

        public NewsControllerGetByIdTests()
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
        public async Task GetById_ReturnsOkResult_WithNews_WhenNewsExists()
        {
            int testId = 9; 
            var expectedNews = new NewsDTO
            {
                Id = testId,
                Title = "Тестова новина за ID",
                URL = "news:string"
            };

            _mediatorMock
                .Setup(m => m.Send(It.Is<GetNewsByIdQuery>(q => q.Id == testId), default))
                .ReturnsAsync(Result.Ok(expectedNews));

            var result = await _controller.GetById(testId);

            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
            okResult.Value.Should().BeEquivalentTo(expectedNews);

            _mediatorMock.Verify(m => m.Send(It.IsAny<GetNewsByIdQuery>(), default), Times.Once);
        }

        [Fact]
        public async Task GetById_ReturnsBadRequestOrNotFound_WhenNewsDoesNotExist()
        {
            int nonExistingId = 99;

            _mediatorMock
                .Setup(m => m.Send(It.Is<GetNewsByIdQuery>(q => q.Id == nonExistingId), default))
                .ReturnsAsync(Result.Fail("News with this ID was not found"));

            var result = await _controller.GetById(nonExistingId);

            result.Should().NotBeOfType<OkObjectResult>();

            _mediatorMock.Verify(m => m.Send(It.IsAny<GetNewsByIdQuery>(), default), Times.Once);
        }
    }
}