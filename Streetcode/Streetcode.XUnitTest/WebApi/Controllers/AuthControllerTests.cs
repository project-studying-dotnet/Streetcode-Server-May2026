using System.Security.Claims;
using FluentAssertions;
using FluentResults;
using Google.Apis.Auth;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Streetcode.BLL.DTO.Users;
using Streetcode.BLL.MediatR.Users.Login;
using Streetcode.BLL.MediatR.Users.LoginGoogle;
using Streetcode.BLL.MediatR.Users.RefreshToken;
using Streetcode.BLL.MediatR.Users.Register;
using Streetcode.DAL.Enums;
using Streetcode.WebApi.Controllers;
using Streetcode.WebApi.Controllers.Users;
using Streetcode.WebApi.Service.Interfaces;
using Xunit;

namespace Streetcode.XUnitTest.WebApi.Controllers;

public class AuthControllerTests
{
    private readonly Mock<IMediator> _mediatorMock;
    private readonly Mock<IGoogleAuthService> _googleAuthServiceMock;
    private readonly AuthController _controller;

    public AuthControllerTests()
    {
        _mediatorMock = new Mock<IMediator>();
        _googleAuthServiceMock = new Mock<IGoogleAuthService>();

        _controller = new AuthController(_googleAuthServiceMock.Object);

        var field = typeof(BaseApiController).GetField(
            "<Mediator>k__BackingField",
            System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);

        if (field != null)
        {
            field.SetValue(_controller, _mediatorMock.Object);
        }
        else
        {
            throw new Exception("Не удалось найти поле <Mediator>k__BackingField в BaseApiController.");
        }
    }

    private static LoginResultDto CreateLoginResult() => new LoginResultDto
    {
        User = new UserDto { Id = 1, Email = "test@mail.com", Name = "John", Surname = "Doe", Login = "jd", Role = UserRole.MainAdministrator },
        Token = "jwt-token",
        RefreshToken = "refresh-token",
        ExpireAt = DateTime.UtcNow.AddHours(1)
    };

    [Fact]
    public async Task Login_ShouldReturnOk()
    {
        var request = new UserLoginDto { Login = "test", Password = "123" };
        var loginResult = CreateLoginResult();

        _mediatorMock.Setup(x => x.Send(It.IsAny<LoginUserCommand>(), It.IsAny<CancellationToken>()))
                     .ReturnsAsync(Result.Ok(loginResult));

        var result = await _controller.Login(request);

        result.Should().BeOfType<OkObjectResult>();
        ((OkObjectResult)result).Value.Should().BeEquivalentTo(loginResult);
    }

    [Fact]
    public async Task Register_ShouldReturnOk()
    {
        // Arrange
        var request = new UserRegisterDto
        {
            Email = "test@mail.com",
            Password = "123",
            PasswordConfirmation = "123",
            Name = "John",
            Surname = "Doe"
        };

        _mediatorMock
           .Setup(x => x.Send(It.IsAny<RegisterUserCommand>(), It.IsAny<CancellationToken>()))
           .ReturnsAsync(Result.Ok());

        // Act
        var result = await _controller.Register(request);

        // Assert
        result.Should().BeOfType<OkObjectResult>();

        _mediatorMock.Verify(x => x.Send(It.IsAny<RegisterUserCommand>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task RefreshToken_ShouldReturnOk()
    {
        var request = new RefreshTokenRequestDto { Token = "tiken", RefreshToken = "token" };
        var loginResult = CreateLoginResult();

        _mediatorMock.Setup(x => x.Send(It.IsAny<RefreshTokenCommand>(), It.IsAny<CancellationToken>()))
                     .ReturnsAsync(Result.Ok(loginResult));

        var result = await _controller.RefreshToken(request);

        result.Should().BeOfType<OkObjectResult>();
        ((OkObjectResult)result).Value.Should().BeEquivalentTo(loginResult);
    }

    [Fact]
    public async Task Logout_ShouldReturnUnauthorized_WhenUserClaimsAreInvalid()
    {
        // Arrange
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal(new ClaimsIdentity()) }
        };

        // Act
        var result = await _controller.Logout(CancellationToken.None);

        // Assert
        result.Should().BeOfType<UnauthorizedResult>();
    }

    [Fact]
    public async Task GoogleLogin_ShouldReturnOk_WhenTokenIsValid()
    {
        var request = new GoogleLoginRequest { IdToken = "valid-token" };
        var payload = new GoogleJsonWebSignature.Payload { Email = "test@test.com", GivenName = "John", FamilyName = "Doe" };
        var loginResult = CreateLoginResult();

        _googleAuthServiceMock.Setup(x => x.ValidateTokenAsync("valid-token")).ReturnsAsync(payload);
        _mediatorMock.Setup(x => x.Send(It.IsAny<GoogleLoginCommand>(), It.IsAny<CancellationToken>()))
                     .ReturnsAsync(Result.Ok(loginResult));

        var result = await _controller.GoogleLogin(request);

        result.Should().BeOfType<OkObjectResult>();
        ((OkObjectResult)result).Value.Should().BeEquivalentTo(loginResult);
    }
}