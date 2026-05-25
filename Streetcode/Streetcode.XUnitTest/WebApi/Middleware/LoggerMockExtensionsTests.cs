using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Streetcode.XUnitTest.WebApi.Middleware
{
    public class LoggerMockExtensionsTests
    {
        [Fact]
        public void VerifyLog_ShouldVerifyLoggedMessage()
        {
            var loggerMock = new Mock<ILogger<object>>();

            loggerMock.Object.Log(
                LogLevel.Information,
                new EventId(0),
                "Test message",
                null,
                (state, exception) => state.ToString()!);

            loggerMock.VerifyLog("Test message");
        }

        [Fact]
        public void VerifyLog_ShouldVerifyWarningLevel()
        {
            var loggerMock = new Mock<ILogger<object>>();

            loggerMock.Object.Log(
                LogLevel.Warning,
                new EventId(1),
                "Warning occurred",
                null,
                (state, exception) => state.ToString()!);

            loggerMock.VerifyLog(
                "Warning occurred",
                LogLevel.Warning);
        }
    }
}
