using FluentAssertions;
using FluentResults;
using Google.Apis.Auth;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Streetcode.Auth.Controllers.Users;
using Streetcode.Auth.MediatR.Users.Login;
using Streetcode.Auth.MediatR.Users.LoginGoogle;
using Streetcode.Auth.MediatR.Users.Logout;
using Streetcode.Auth.MediatR.Users.RefreshToken;
using Streetcode.Auth.MediatR.Users.Register;
using Streetcode.Auth.Models.DTO;
using Streetcode.Auth.Services.Interfaces.Users;
using Streetcode.Common.Enums;
using Xunit;

namespace Streetcode.XUnitTest.AuthService.Controllers;

public class AuthControllerTests
{
    private readonly Mock<IMediator> _mediatorMock;
    private readonly Mock<IGoogleAuthService> _googleAuthServiceMock;
    private readonly AuthController _controller;

    public AuthControllerTests()
    {
        _mediatorMock = new Mock<IMediator>();
        _googleAuthServiceMock = new Mock<IGoogleAuthService>();
        _controller = new AuthController(_mediatorMock.Object, _googleAuthServiceMock.Object);
    }

    [Fact]
    public async Task Login_ShouldReturnOk()
    {
        // Arrange
        var request = new Auth.Models.DTO.UserLoginDto
        {
            Login = "test",
            Password = "123"
        };

        var authResult = new AuthResponseDto
        {
            User = new Auth.Models.DTO.UserDto
            {
                Id = 1,
                Email = "test@mail.com",
                Name = "John",
                Surname = "Doe"
            },
            Token = "jwt-token",
            RefreshToken = "refresh-token",
            ExpireAt = DateTime.UtcNow.AddHours(1)
        };

        var expected = Result.Ok(authResult);

        _mediatorMock
            .Setup(x => x.Send(
                It.IsAny<LoginUserCommand>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(expected);

        // Act
        var result = await _controller.Login(request);

        // Assert
        result.Should().BeOfType<OkObjectResult>();

        _mediatorMock.Verify(
            x => x.Send(It.IsAny<LoginUserCommand>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Register_ShouldReturnOk()
    {
        var request = new UserRegisterDto
        {
            Email = "test@mail.com",
            Password = "123",
            PasswordConfirmation = "123",
            Name = "John",
            Surname = "Doe"
        };
        var authResult = new AuthResponseDto
        {
            User = new UserDto
            {
                Id = 1,
                Email = "test@mail.com",
                Name = "John",
                Surname = "Doe"
            },
            Token = "jwt-token",
            RefreshToken = "refresh-token",
            ExpireAt = DateTime.UtcNow.AddHours(1)
        };

        var expected = Result.Ok(authResult);

        _mediatorMock
            .Setup(x => x.Send(
                It.IsAny<RegisterUserCommand>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(expected);

        // Act
        var result = await _controller.Register(request);

        result.Should().BeOfType<OkObjectResult>();

        _mediatorMock.Verify(
            x => x.Send(It.IsAny<RegisterUserCommand>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task RefreshToken_ShouldReturnOk()
    {
        var request = new RefreshTokenRequestDto
        {
            RefreshToken = "token"
        };

        var authResult = new AuthResponseDto
        {
            User = new UserDto
            {
                Id = 1,
                Email = "test@mail.com",
                Name = "John",
                Surname = "Doe"
            },
            Token = "jwt-token",
            RefreshToken = "refresh-token",
            ExpireAt = DateTime.UtcNow.AddHours(1)
        };

        _mediatorMock
            .Setup(x => x.Send(
                It.IsAny<RefreshTokenCommand>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Ok(authResult));

        var result = await _controller.RefreshToken(request);

        result.Should().BeOfType<OkObjectResult>();

        _mediatorMock.Verify(
            x => x.Send(It.IsAny<RefreshTokenCommand>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Logout_ShouldReturnBadRequest_WhenTokenIsNullOrEmpty()
    {
        // Act:
        var result = await _controller.Logout(null!);

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
        _mediatorMock.Verify(x => x.Send(It.IsAny<LogoutUserCommand>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Logout_ShouldReturnOk_WhenTokenIsValid()
    {
        string refreshToken = "test-refresh-token";

        _mediatorMock
            .Setup(x => x.Send(
                It.Is<LogoutUserCommand>(c => c.RefreshToken == refreshToken),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Ok());

        var result = await _controller.Logout(refreshToken);

        result.Should().BeOfType<NotFoundResult>();

        _mediatorMock.Verify(
            x => x.Send(
                It.Is<LogoutUserCommand>(c => c.RefreshToken == refreshToken),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task GoogleLogin_ShouldReturnOk_WhenTokenIsValid()
    {
        // Arrange
        var request = new GoogleLoginRequest { IdToken = "valid-token" };
        var payload = new GoogleJsonWebSignature.Payload { Email = "test@test.com", GivenName = "John", FamilyName = "Doe" };

        var authResponse = new AuthResponseDto
        {
            User = new UserDto
            {
                Id = 1,
                Name = "John",
                Surname = "Doe",
                Email = "john@example.com",
                Login = "johndoe",
                Role = UserRole.MainAdministrator
            },
            Token = "jwt-token",
            RefreshToken = "refresh-token",
            ExpireAt = DateTime.UtcNow.AddHours(1)
        };

        _googleAuthServiceMock.Setup(x => x.ValidateTokenAsync("valid-token"))
            .ReturnsAsync(payload);

        _mediatorMock.Setup(x => x.Send(It.IsAny<GoogleLoginCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Ok(authResponse));

        var result = await _controller.GoogleLogin(request);

        // Assert
        result.Should().BeOfType<OkObjectResult>();

        var okResult = result as OkObjectResult;
        okResult?.Value.Should().BeEquivalentTo(authResponse);

        _mediatorMock.Verify(
            x => x.Send(It.IsAny<GoogleLoginCommand>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }
}