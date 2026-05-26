using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Streetcode.BLL.Exceptions;
using Streetcode.WebApi.Middleware;
using Xunit;

namespace Streetcode.XUnitTest.WebApi.Middleware;

public class ExceptionMiddlewareTests
{
    private readonly Mock<ILogger<ExceptionMiddleware>> _loggerMock;
    private readonly Mock<IConfiguration> _configMock;

    public ExceptionMiddlewareTests()
    {
        _loggerMock = new Mock<ILogger<ExceptionMiddleware>>();
        _configMock = new Mock<IConfiguration>();
    }

    [Theory]
    [InlineData(typeof(NotFoundException), HttpStatusCode.NotFound)]
    [InlineData(typeof(ForbiddenException), HttpStatusCode.Forbidden)]
    [InlineData(typeof(UnauthorizedAccessException), HttpStatusCode.Unauthorized)]
    public async Task InvokeAsync_ShouldReturnExpectedStatusCode_WhenKnownExceptionThrown(
        Type exceptionType,
        HttpStatusCode expectedStatus)
    {
        var exception = (Exception)Activator.CreateInstance(exceptionType, "Error message") !;
        var middleware = CreateMiddleware(exception, false);
        var context = CreateContext();

        await middleware.InvokeAsync(context);

        Assert.Equal((int)expectedStatus, context.Response.StatusCode);
        Assert.True(context.Response.Headers.ContainsKey("X-Correlation-Id"));
    }

    [Fact]
    public async Task InvokeAsync_ShouldHideDetails_WhenShowExceptionDetailsIsFalse()
    {
        var middleware = CreateMiddleware(
            new Exception("Secret details"),
            showDetails: false);

        var context = CreateContext();

        await middleware.InvokeAsync(context);

        context.Response.Body.Seek(0, SeekOrigin.Begin);
        var body = await new StreamReader(context.Response.Body).ReadToEndAsync();

        var problem = JsonSerializer.Deserialize<JsonElement>(body);

        Assert.Equal(500, context.Response.StatusCode);
        Assert.Equal("An internal error occurred.", problem.GetProperty("detail").GetString());
    }

    [Fact]
    public async Task InvokeAsync_ShouldShowDetails_WhenShowExceptionDetailsIsTrue()
    {
        var middleware = CreateMiddleware(
            new Exception("Secret details"),
            showDetails: true);

        var context = CreateContext();

        await middleware.InvokeAsync(context);

        context.Response.Body.Seek(0, SeekOrigin.Begin);
        var body = await new StreamReader(context.Response.Body).ReadToEndAsync();

        Assert.Contains("Secret details", body);
    }

    private ExceptionMiddleware CreateMiddleware(Exception ex, bool showDetails)
    {
        _configMock.Setup(c => c["ShowExceptionDetails"])
            .Returns(showDetails.ToString());

        RequestDelegate next = _ => throw ex;

        return new ExceptionMiddleware(next, _loggerMock.Object, _configMock.Object);
    }

    private static DefaultHttpContext CreateContext()
    {
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();
        context.Request.Path = "/test";
        context.TraceIdentifier = "trace-123";
        return context;
    }
}