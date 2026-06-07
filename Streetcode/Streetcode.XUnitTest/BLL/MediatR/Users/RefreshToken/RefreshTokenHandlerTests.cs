using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Streetcode.BLL.DTO.Users;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.Interfaces.Users;
using Streetcode.BLL.MediatR.Users.RefreshToken;
using Streetcode.BLL.Resources;
using Streetcode.BLL.Settings;
using Streetcode.DAL.Entities.Users;
using Streetcode.DAL.Enums;
using Xunit;

namespace Streetcode.XUnitTest.BLL.MediatR.Users.RefreshToken;

public class RefreshTokenHandlerTests
{
    private const int UserId = 1;
    private const string AccessToken = "access-token";
    private const string RefreshToken = "refresh-token";
    private const string OldRefreshToken = "old-refresh-token";
    private const string NewRefreshToken = "new-refresh-token";
    private const string WrongRefreshToken = "wrong-refresh-token";
    private const string ActualRefreshToken = "actual-refresh-token";
    private const string AdminUserName = "admin";
    private const string AdminName = "Admin";
    private const string AdminSurname = "User";
    private readonly Mock<UserManager<User>> _userManagerMock;
    private readonly Mock<ILoggerService> _loggerMock;
    private readonly Mock<ITokenService> _tokenServiceMock;
    private readonly JwtSettings _jwtSettings;
    private readonly RefreshTokenHandler _handler;

    public RefreshTokenHandlerTests()
    {
        var userStoreMock = new Mock<IUserStore<User>>();

        _userManagerMock = new Mock<UserManager<User>>(
            userStoreMock.Object,
            Mock.Of<IOptions<IdentityOptions>>(),
            Mock.Of<IPasswordHasher<User>>(),
            Array.Empty<IUserValidator<User>>(),
            Array.Empty<IPasswordValidator<User>>(),
            Mock.Of<ILookupNormalizer>(),
            new IdentityErrorDescriber(),
            Mock.Of<IServiceProvider>(),
            Mock.Of<ILogger<UserManager<User>>>());

        _loggerMock = new Mock<ILoggerService>();
        _tokenServiceMock = new Mock<ITokenService>();

        _jwtSettings = new JwtSettings
        {
            Key = "super-secret-key-super-secret-key",
            Issuer = "test-issuer",
            Audience = "test-audience",
            AccessTokenLifetimeInMinutes = 120,
            RefreshTokenLifetimeInDays = 7,
        };

        _handler = new RefreshTokenHandler(
            _userManagerMock.Object,
            _loggerMock.Object,
            _tokenServiceMock.Object,
            _jwtSettings);
    }

    [Fact]
    public async Task Handle_ShouldReturnOk_WhenRefreshTokenIsValid()
    {
        // Arrange
        const int userId = UserId;
        const string oldRefreshToken = OldRefreshToken;
        const string newRefreshToken = NewRefreshToken;

        var requestDto = new RefreshTokenRequestDto
        {
            Token = AccessToken,
            RefreshToken = oldRefreshToken,
        };

        var command = new RefreshTokenCommand(requestDto);

        var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new[]
        {
            new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
        }));

        var user = new User
        {
            Id = userId,
            UserName = AdminUserName,
            Name = AdminName,
            Surname = AdminSurname,
            Role = UserRole.MainAdministrator,
            RefreshToken = oldRefreshToken,
            RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(1),
        };

        var jwtToken = new JwtSecurityToken(
            issuer: _jwtSettings.Issuer,
            audience: _jwtSettings.Audience,
            claims: new[]
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.Role, user.Role.ToString()),
            },
            expires: DateTime.UtcNow.AddMinutes(30));

        _tokenServiceMock
            .Setup(x => x.GetPrincipalFromExpiredToken(requestDto.Token))
            .Returns(claimsPrincipal);

        _userManagerMock
            .Setup(x => x.FindByIdAsync(userId.ToString()))
            .ReturnsAsync(user);

        _tokenServiceMock
            .Setup(x => x.GenerateJWTToken(user))
            .Returns(jwtToken);

        _tokenServiceMock
            .Setup(x => x.GenerateRefreshToken())
            .Returns(newRefreshToken);

        _userManagerMock
            .Setup(x => x.UpdateAsync(user))
            .ReturnsAsync(IdentityResult.Success);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();

        result.Value.Token.Should().NotBeNullOrWhiteSpace();
        result.Value.RefreshToken.Should().Be(newRefreshToken);
        result.Value.User.Id.Should().Be(userId);
        result.Value.User.Login.Should().Be(AdminUserName);
        result.Value.User.Role.Should().Be(UserRole.MainAdministrator);

        user.RefreshToken.Should().Be(newRefreshToken);
        user.RefreshTokenExpiryTime.Should().BeAfter(DateTime.UtcNow);

        _userManagerMock.Verify(x => x.UpdateAsync(user), Times.Once);
        _loggerMock.Verify(x => x.LogInformation(string.Format(ErrorMessages.SuccessfulRefreshToken, user.Id)), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldReturnFail_WhenTokenDoesNotContainUserId()
    {
        // Arrange
        var requestDto = new RefreshTokenRequestDto
        {
            Token = AccessToken,
            RefreshToken = RefreshToken,
        };

        var command = new RefreshTokenCommand(requestDto);

        var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity());

        _tokenServiceMock
            .Setup(x => x.GetPrincipalFromExpiredToken(requestDto.Token))
            .Returns(claimsPrincipal);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors[0].Message.Should().Be(ErrorMessages.InvalidToken);

        _userManagerMock.Verify(x => x.FindByIdAsync(It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldReturnFail_WhenUserDoesNotExist()
    {
        // Arrange
        const int userId = UserId;

        var requestDto = new RefreshTokenRequestDto
        {
            Token = AccessToken,
            RefreshToken = RefreshToken,
        };

        var command = new RefreshTokenCommand(requestDto);

        var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new[]
        {
            new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
        }));

        _tokenServiceMock
            .Setup(x => x.GetPrincipalFromExpiredToken(requestDto.Token))
            .Returns(claimsPrincipal);

        _userManagerMock
            .Setup(x => x.FindByIdAsync(userId.ToString()))
            .ReturnsAsync((User?)null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors[0].Message.Should().Be(ErrorMessages.InvalidRefreshToken);

        _tokenServiceMock.Verify(x => x.GenerateJWTToken(It.IsAny<User>()), Times.Never);
        _userManagerMock.Verify(x => x.UpdateAsync(It.IsAny<User>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldReturnFail_WhenRefreshTokenIsInvalid()
    {
        // Arrange
        const int userId = UserId;

        var requestDto = new RefreshTokenRequestDto
        {
            Token = AccessToken,
            RefreshToken = WrongRefreshToken,
        };

        var command = new RefreshTokenCommand(requestDto);

        var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new[]
        {
            new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
        }));

        var user = new User
        {
            Id = userId,
            UserName = AdminUserName,
            Name = AdminName,
            Surname = AdminSurname,
            Role = UserRole.MainAdministrator,
            RefreshToken = ActualRefreshToken,
            RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(1),
        };

        _tokenServiceMock
            .Setup(x => x.GetPrincipalFromExpiredToken(requestDto.Token))
            .Returns(claimsPrincipal);

        _userManagerMock
            .Setup(x => x.FindByIdAsync(userId.ToString()))
            .ReturnsAsync(user);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors[0].Message.Should().Be(ErrorMessages.InvalidRefreshToken);

        _tokenServiceMock.Verify(x => x.GenerateJWTToken(It.IsAny<User>()), Times.Never);
        _userManagerMock.Verify(x => x.UpdateAsync(It.IsAny<User>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldReturnFail_WhenRefreshTokenIsExpired()
    {
        // Arrange
        const int userId = UserId;
        const string refreshToken = RefreshToken;

        var requestDto = new RefreshTokenRequestDto
        {
            Token = AccessToken,
            RefreshToken = refreshToken,
        };

        var command = new RefreshTokenCommand(requestDto);

        var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new[]
        {
            new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
        }));

        var user = new User
        {
            Id = userId,
            UserName = AdminUserName,
            Name = AdminName,
            Surname = AdminSurname,
            Role = UserRole.MainAdministrator,
            RefreshToken = refreshToken,
            RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(-1),
        };

        _tokenServiceMock
            .Setup(x => x.GetPrincipalFromExpiredToken(requestDto.Token))
            .Returns(claimsPrincipal);

        _userManagerMock
            .Setup(x => x.FindByIdAsync(userId.ToString()))
            .ReturnsAsync(user);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors[0].Message.Should().Be(ErrorMessages.InvalidRefreshToken);

        _tokenServiceMock.Verify(x => x.GenerateJWTToken(It.IsAny<User>()), Times.Never);
        _userManagerMock.Verify(x => x.UpdateAsync(It.IsAny<User>()), Times.Never);
    }
}
