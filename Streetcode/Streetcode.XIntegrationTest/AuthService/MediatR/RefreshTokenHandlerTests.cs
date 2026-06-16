using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using AutoMapper;
using Moq;
using Streetcode.Auth.MediatR.Users.RefreshToken;
using Streetcode.Auth.Models.DTO;
using Streetcode.Auth.Models.Entities;
using Streetcode.Auth.Services.Interfaces.Users;
using Xunit;

namespace Streetcode.XUnitTest.AuthService.MediatR
{
    public class RefreshTokenHandlerTests
    {
        [Fact]
        public async Task Handle_ShouldReturnSuccess_WhenRefreshIsValid()
        {
            // Arrange
            var refreshService = new Mock<IRefreshTokenService>();
            var jwtService = new Mock<IJwtTokenService>();
            var mapper = new Mock<IMapper>();

            var user = new User
            {
                Id = 1,
                Email = "test@test.com",
                UserName = "test@test.com",
                Name = "Test",
                Surname = "User",
                Role = Streetcode.Common.Enums.UserRole.Moderator
            };

            var jwt = new JwtSecurityToken(
                claims: new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, "1")
                },
                expires: DateTime.UtcNow.AddMinutes(10)
            );

            refreshService
                .Setup(x => x.RefreshAsync(It.IsAny<string>()))
                .ReturnsAsync((user, "new_refresh_token"));

            jwtService
                .Setup(x => x.GenerateToken(user))
                .Returns(jwt);

            mapper
                .Setup(x => x.Map<UserDto>(user))
                .Returns(new UserDto
                {
                    Id = 1,
                    Email = "test@test.com"
                });

            var handler = new RefreshTokenHandler(
                refreshService.Object,
                jwtService.Object,
                mapper.Object);

            var command = new RefreshTokenCommand(
                new RefreshTokenRequestDto
                {
                    RefreshToken = "old_token"
                });

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal("new_refresh_token", result.Value.RefreshToken);
            Assert.NotNull(result.Value.Token);
        }

        [Fact]
        public async Task Handle_ShouldReturnFailure_WhenTokenIsInvalid()
        {
            // Arrange
            var refreshService = new Mock<IRefreshTokenService>();
            var jwtService = new Mock<IJwtTokenService>();
            var mapper = new Mock<IMapper>();

            refreshService
                .Setup(x => x.RefreshAsync(It.IsAny<string>()))
                .ThrowsAsync(new Microsoft.IdentityModel.Tokens.SecurityTokenException("Invalid token"));

            var handler = new RefreshTokenHandler(
                refreshService.Object,
                jwtService.Object,
                mapper.Object);

            var command = new RefreshTokenCommand(
                new RefreshTokenRequestDto
                {
                    RefreshToken = "bad_token"
                });

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result.IsFailed);
            Assert.Contains(result.Errors, e =>
                e.Message == "Invalid token");
        }
    }
}