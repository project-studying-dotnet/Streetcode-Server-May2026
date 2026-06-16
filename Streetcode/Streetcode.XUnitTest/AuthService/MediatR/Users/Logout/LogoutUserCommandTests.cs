using FluentAssertions;
using Streetcode.Auth.MediatR.Users.Logout;
using Xunit;

namespace Streetcode.XUnitTest.AuthService.MediatR.Users.Logout
{
    public class LogoutUserCommandTests
    {
        [Fact]
        public void Constructor_ShouldSetRefreshToken()
        {
            // Arrange
            const string token = "test-refresh-token";

            // Act
            var command = new LogoutUserCommand(token);

            // Assert
            command.RefreshToken.Should().Be(token);
        }
    }
}