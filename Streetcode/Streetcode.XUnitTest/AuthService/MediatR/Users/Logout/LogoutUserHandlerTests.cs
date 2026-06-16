using FluentAssertions;
using Moq;
using Streetcode.Auth.MediatR.Users.Logout;
using Streetcode.Auth.Services.Interfaces.Logging;
using Streetcode.Auth.Services.Interfaces.Users;
using Xunit;

namespace Streetcode.XUnitTest.AuthService.MediatR.Users.Logout
{
    public class LogoutUserHandlerTests
    {
        private readonly Mock<IRefreshTokenService> _tokenServiceMock;
        private readonly Mock<ILoggerService> _loggerMock;
        private readonly LogoutUserHandler _handler;

        public LogoutUserHandlerTests()
        {
            _tokenServiceMock = new Mock<IRefreshTokenService>();
            _loggerMock = new Mock<ILoggerService>();

            _handler = new LogoutUserHandler(_tokenServiceMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task Handle_ValidRefreshToken_ReturnsSuccess()
        {
            // Arrange
            var command = new LogoutUserCommand("valid-token");
            _tokenServiceMock.Setup(s => s.RevokeAsync(command.RefreshToken)).Returns(Task.CompletedTask);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            _tokenServiceMock.Verify(s => s.RevokeAsync(command.RefreshToken), Times.Once);
            _loggerMock.Verify(l => l.LogInformation(It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async Task Handle_ServiceThrowsException_ReturnsFail()
        {
            // Arrange
            var command = new LogoutUserCommand("invalid-token");
            _tokenServiceMock
                .Setup(s => s.RevokeAsync(It.IsAny<string>()))
                .ThrowsAsync(new Exception("Revocation failed"));

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsFailed.Should().BeTrue();
            result.Errors.Should().ContainSingle(e => e.Message == "Failed to revoke session.");
            _loggerMock.Verify(l => l.LogError(command, It.Is<string>(s => s.Contains("Logout failed"))), Times.Once);
        }
    }
}