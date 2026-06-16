using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Logging;
using Moq;

namespace Streetcode.XUnitTest.SharedWebService.Middleware
{
    public static class LoggerMockExtensions
    {
        [SuppressMessage("Performance", "CA1873:Do not directly use a method that returns a value as an argument to a logger", Justification = "Necessary for verifying log message content in unit tests.")]
        public static void VerifyLog<T>(
            this Mock<ILogger<T>> logger,
            string expectedMessage,
            LogLevel level = LogLevel.Information)
        {
            logger.Verify(
                x => x.Log(
                    level,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains(expectedMessage)),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.AtLeastOnce);
        }
    }
}
