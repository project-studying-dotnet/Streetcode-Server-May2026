using Moq;
using Streetcode.Auth.MediatR.Users.Logout;
using Streetcode.Auth.Services.Interfaces.Logging;
using Streetcode.Auth.Services.Interfaces.Users;
using Xunit;

namespace Streetcode.XUnitTest.AuthService.MediatR
{
    public class LogoutUserHandlerTests
    {
        [Fact]
        public async Task Handle_ShouldReturnSuccess_WhenTokenRevoked()
        {
            // Arrange
            var tokenService = new Mock<IRefreshTokenService>();
            var logger = new Mock<ILoggerService>();

            tokenService
                .Setup(x => x.RevokeAsync(It.IsAny<string>()))
                .Returns(Task.CompletedTask);

            var handler = new LogoutUserHandler(
                tokenService.Object,
                logger.Object);

            var command = new LogoutUserCommand("refresh_token");

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);

            tokenService.Verify(
                x => x.RevokeAsync("refresh_token"),
                Times.Once);

            logger.Verify(
                x => x.LogInformation("User session revoked successfully."),
                Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldReturnFailure_WhenExceptionOccurs()
        {
            // Arrange
            var tokenService = new Mock<IRefreshTokenService>();
            var logger = new Mock<ILoggerService>();

            tokenService
                .Setup(x => x.RevokeAsync(It.IsAny<string>()))
                .ThrowsAsync(new Exception("Database error"));

            var handler = new LogoutUserHandler(
                tokenService.Object,
                logger.Object);

            var command = new LogoutUserCommand("refresh_token");

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result.IsFailed);

            Assert.Contains(
                result.Errors,
                e => e.Message == "Failed to revoke session.");

            tokenService.Verify(
                x => x.RevokeAsync("refresh_token"),
                Times.Once);

            logger.Verify(
                x => x.LogError(
                    command,
                    It.Is<string>(msg => msg.Contains("Logout failed"))),
                Times.Once);
        }
    }
}