using System.IdentityModel.Tokens.Jwt;
using FluentAssertions;
using Microsoft.IdentityModel.Tokens;
using Moq;
using Streetcode.Auth.MediatR.Users.RefreshToken;
using Streetcode.Auth.Models.DTO;
using Streetcode.Auth.Models.Entities;
using Streetcode.Auth.Services.Interfaces.Users;
using Xunit;

namespace Streetcode.XUnitTest.AuthService.MediatR.Users.RefreshToken;

public class RefreshTokenHandlerTests
{
    private readonly Mock<IRefreshTokenService> _refreshServiceMock;
    private readonly Mock<IJwtTokenService> _jwtServiceMock;
    private readonly Mock<AutoMapper.IMapper> _mapperMock;
    private readonly RefreshTokenHandler _handler;

    public RefreshTokenHandlerTests()
    {
        _refreshServiceMock = new Mock<IRefreshTokenService>();
        _jwtServiceMock = new Mock<IJwtTokenService>();
        _mapperMock = new Mock<AutoMapper.IMapper>();

        _handler = new RefreshTokenHandler(
            _refreshServiceMock.Object,
            _jwtServiceMock.Object,
            _mapperMock.Object);
    }

    [Fact]
    public async Task Handle_ValidRequest_ReturnsSuccessResult()
    {
        // Arrange
        var user = new User
        {
            Id = 1,
            UserName = "testUser",
            Name = "John",
            Surname = "Doe"
        };
        var userDto = new UserDto { Id = 1 };
        var newRefreshToken = "new-refresh-token";
        var request = new RefreshTokenCommand(new RefreshTokenRequestDto { RefreshToken = "old-token" });

        var jwtToken = new JwtSecurityToken(expires: DateTime.UtcNow.AddMinutes(30));

        _refreshServiceMock.Setup(s => s.RefreshAsync("old-token"))
            .ReturnsAsync((user, newRefreshToken));

        _jwtServiceMock.Setup(s => s.GenerateToken(user)).Returns(jwtToken);
        _mapperMock.Setup(m => m.Map<UserDto>(user)).Returns(userDto);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.RefreshToken.Should().Be(newRefreshToken);
        result.Value.User.Should().Be(userDto);
        result.Value.Token.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task Handle_InvalidToken_ReturnsFailResult()
    {
        // Arrange
        var request = new RefreshTokenCommand(new RefreshTokenRequestDto { RefreshToken = "bad-token" });

        _refreshServiceMock.Setup(s => s.RefreshAsync(It.IsAny<string>()))
            .ThrowsAsync(new SecurityTokenException("Invalid token"));

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainSingle(e => e.Message == "Invalid token");
    }
}