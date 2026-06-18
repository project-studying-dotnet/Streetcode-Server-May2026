using FluentAssertions;
using Streetcode.Auth.MediatR.Users.RefreshToken;
using Streetcode.Auth.Models.DTO;
using Xunit;

namespace Streetcode.XUnitTest.AuthService.MediatR.Users.RefreshToken;

public class RefreshTokenCommandTests
{
    [Fact]
    public void RefreshTokenCommand_ShouldHaveCorrectProperties()
    {
        // Arrange
        var request = new RefreshTokenRequestDto
        {
            RefreshToken = "refresh-token"
        };

        // Act
        var command = new RefreshTokenCommand(request);

        // Assert
        command.RefreshTokenRequest.RefreshToken.Should().Be("refresh-token");
    }
}