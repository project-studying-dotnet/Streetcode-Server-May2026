using FluentAssertions;
using Streetcode.Auth.MediatR.Users.Login;
using Streetcode.Auth.Models.DTO;
using Xunit;

namespace Streetcode.XUnitTest.AuthService.MediatR.Users.Login
{
    public class LoginUserCommandTests
    {
        [Fact]
        public void Constructor_ShouldSetLoginRequest()
        {
            // Arrange
            var loginDto = new UserLoginDto
            {
                Login = "testUser",
                Password = "password123"
            };

            // Act
            var command = new LoginUserCommand(loginDto);

            // Assert
            command.loginRequest.Should().NotBeNull();
            command.loginRequest.Login.Should().Be("testUser");
            command.loginRequest.Password.Should().Be("password123");
        }
    }
}