using FluentAssertions;
using Streetcode.BLL.DTO.Users;
using Streetcode.BLL.MediatR.Users.RefreshToken;
using Xunit;

namespace Streetcode.XUnitTest.BLL.MediatR.Users.RefreshToken;

public class RefreshTokenCommandTests
{
    [Fact]
    public void Constructor_ShouldSetRefreshTokenRequest()
    {
        // Arrange
        var request = new RefreshTokenRequestDto
        {
            Token = "access-token",
            RefreshToken = "refresh-token",
        };

        // Act
        var command = new RefreshTokenCommand(request);

        // Assert
        command.RefreshTokenRequest.Should().Be(request);
    }
}