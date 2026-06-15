using Moq;
using Serilog;
using Streetcode.Auth.Services.Services.Logging;
using Xunit;

public class LoggerServiceTests
{
    private Mock<ILogger> GetLoggerMock()
        => new Mock<ILogger>();

    [Fact]
    public void LogInformation_ShouldCallSerilogInformation()
    {
        var logger = GetLoggerMock();
        var service = new LoggerService(logger.Object);

        service.LogInformation("test");

        logger.Verify(x => x.Information("test"), Times.Once);
    }

    [Fact]
    public void LogWarning_ShouldCallSerilogWarning()
    {
        var logger = GetLoggerMock();
        var service = new LoggerService(logger.Object);

        service.LogWarning("warning");

        logger.Verify(x => x.Warning("warning"), Times.Once);
    }

    [Fact]
    public void LogDebug_ShouldCallSerilogDebug()
    {
        var logger = GetLoggerMock();
        var service = new LoggerService(logger.Object);

        service.LogDebug("debug");

        logger.Verify(x => x.Debug("debug"), Times.Once);
    }

    [Fact]
    public void LogTrace_ShouldCallVerbose()
    {
        var logger = GetLoggerMock();
        var service = new LoggerService(logger.Object);

        service.LogTrace("trace");

        logger.Verify(x => x.Verbose("trace"), Times.Once);
    }

    [Fact]
    public void LogError_ShouldFormatMessageCorrectly()
    {
        var logger = GetLoggerMock();
        var service = new LoggerService(logger.Object);
        var request = new TestRequest();

        string expectedClassName = request.GetType().Name;

        service.LogError(request, "something went wrong");

        logger.Verify(
            x => x.Error(
                "{RequestClass} handled with the error: {ErrorMsg}",
                It.IsAny<string>(),
                "something went wrong"),
            Times.Once);
    }

    [Fact]
    public void LogError_ShouldHandleNullRequest()
    {
        var logger = GetLoggerMock();
        var service = new LoggerService(logger.Object);

        service.LogError(null, "error happened");

        logger.Verify(
            x => x.Error(
                "UnknownRequest handled with the error: {ErrorMsg}",
                "error happened"),
            Times.Once);
    }

    private class TestRequest { }
}