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

            loggerMock
                .Setup(x => x.IsEnabled(LogLevel.Information))
                .Returns(true);

            var nextMock = new Mock<RequestDelegate>();
            var context = new DefaultHttpContext();

            var middleware = new RequestLoggingMiddleware(nextMock.Object, loggerMock.Object);

            await middleware.InvokeAsync(context);

            loggerMock.VerifyLog("Started", LogLevel.Information);
            loggerMock.VerifyLog("Finished", LogLevel.Information);
        }

        [Fact]
        public async Task InvokeAsync_ShouldLogCorrectStatusCode()
        {
            var loggerMock = new Mock<ILogger<RequestLoggingMiddleware>>();

            loggerMock.Setup(x => x.IsEnabled(LogLevel.Information)).Returns(true);

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
