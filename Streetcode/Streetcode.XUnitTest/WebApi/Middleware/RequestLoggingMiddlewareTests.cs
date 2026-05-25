using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using Streetcode.WebApi.Middleware;
using Xunit;

namespace Streetcode.XUnitTest.WebApi.Middleware
{
    public class RequestLoggingMiddlewareTests
    {
        [Fact]
        public async Task InvokeAsync_ShouldLogRequestAndResponse()
        {
            var loggerMock = new Mock<ILogger<RequestLoggingMiddleware>>();
            var nextMock = new Mock<RequestDelegate>();

            var middleware = new RequestLoggingMiddleware(nextMock.Object, loggerMock.Object);
            var context = new DefaultHttpContext();
            context.Request.Method = "GET";
            context.Request.Path = "/api/test";

            await middleware.InvokeAsync(context);

            loggerMock.VerifyLog("Started GET /api/test", LogLevel.Information);
            loggerMock.VerifyLog("Finished GET /api/test", LogLevel.Information);

            nextMock.Verify(x => x(context), Times.Once);
        }

        [Fact]
        public async Task InvokeAsync_ShouldLogCorrectStatusCode()
        {
            var loggerMock = new Mock<ILogger<RequestLoggingMiddleware>>();
            var nextMock = new Mock<RequestDelegate>();
            nextMock.Setup(n => n(It.IsAny<HttpContext>()))
                    .Callback<HttpContext>(ctx => ctx.Response.StatusCode = 404)
                    .Returns(Task.CompletedTask);

            var middleware = new RequestLoggingMiddleware(nextMock.Object, loggerMock.Object);
            var context = new DefaultHttpContext();

            await middleware.InvokeAsync(context);

            loggerMock.VerifyLog("with status 404", LogLevel.Information);
        }
    }
}