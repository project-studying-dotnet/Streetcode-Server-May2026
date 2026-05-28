using Microsoft.AspNetCore.Http;
using Streetcode.WebApi.Middleware;
using Xunit;

namespace Streetcode.XUnitTest.WebApi.Middleware
{
    public class CorrelationIdMiddlewareTests
    {
        [Fact]
        public async Task InvokeAsync_ShouldAddCorrelationIdToResponse_WhenMissingInRequest()
        {
            var context = new DefaultHttpContext();
            var middleware = new CorrelationIdMiddleware(next: (innerHttpContext) => Task.CompletedTask);

            await middleware.InvokeAsync(context);

            Assert.True(context.Response.Headers.ContainsKey("X-Correlation-Id"));
            Assert.Equal(context.TraceIdentifier, context.Response.Headers["X-Correlation-Id"]);
        }

        [Fact]
        public async Task InvokeAsync_ShouldUseProvidedCorrelationId_WhenPresentInRequest()
        {
            var context = new DefaultHttpContext();
            var expectedId = "custom-correlation-id-123";

            context.Request.Headers["X-Correlation-Id"] = expectedId;

            var middleware = new CorrelationIdMiddleware(
                next: (innerHttpContext) => Task.CompletedTask);

            await middleware.InvokeAsync(context);

            Assert.Equal(
                expectedId,
                context.Response.Headers["X-Correlation-Id"]);

            Assert.Equal(
                expectedId,
                context.Items["CorrelationId"]?.ToString());
        }
    }
}