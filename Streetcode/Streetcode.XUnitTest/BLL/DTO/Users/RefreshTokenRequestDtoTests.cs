using FluentAssertions;
using Streetcode.BLL.DTO.Users;
using Xunit;

namespace Streetcode.XUnitTest.BLL.DTO.Users;

public class RefreshTokenRequestDtoTests
{
    private const string AccessToken = "access-token";
    private const string RefreshToken = "refresh-token";

    [Fact]
    public void Constructor_ShouldCreateRefreshTokenRequestDto_WithCorrectValues()
    {
        // Act
        var dto = new RefreshTokenRequestDto
        {
            Token = AccessToken,
            RefreshToken = RefreshToken,
        };

        // Assert
        dto.Token.Should().Be(AccessToken);
        dto.RefreshToken.Should().Be(RefreshToken);
    }
}