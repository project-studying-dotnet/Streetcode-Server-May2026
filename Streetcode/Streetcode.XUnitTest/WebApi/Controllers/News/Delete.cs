using FluentAssertions;
using FluentResults;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MediatR;
using Moq;
using Streetcode.BLL.MediatR.Newss.Delete;
using Streetcode.WebApi.Controllers;
using Xunit;

namespace Streetcode.XUnitTest.Controllers
{
    public class Delete
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly NewsController _controller;

        public Delete()
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
        public async Task Delete_ReturnsOkResult_WhenSuccessful()
        {
            int testId = 1;

            _mediatorMock
                .Setup(m => m.Send(It.Is<DeleteNewsCommand>(c => c.Id == testId), default))
                .ReturnsAsync(Result.Ok(Unit.Value));

            var result = await _controller.Delete(testId);

            result.Should().BeOfType<OkObjectResult>();

            _mediatorMock.Verify(m => m.Send(It.IsAny<DeleteNewsCommand>(), default), Times.Once);
        }

        [Fact]
        public async Task Delete_ReturnsBadRequestOrNotFound_WhenRequestFails()
        {
            int testId = 1;

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<DeleteNewsCommand>(), default))
                .ReturnsAsync(Result.Fail<Unit>("Failed to delete news"));

            var result = await _controller.Delete(testId);

            result.Should().NotBeOfType<OkObjectResult>();

            _mediatorMock.Verify(m => m.Send(It.IsAny<DeleteNewsCommand>(), default), Times.Once);
        }
    }
}