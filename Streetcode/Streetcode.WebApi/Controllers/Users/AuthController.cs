using System.Diagnostics.CodeAnalysis;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Streetcode.BLL.DTO.Users;
using Streetcode.BLL.MediatR.Users.Login;
using Streetcode.BLL.MediatR.Users.LoginGoogle;
using Streetcode.BLL.MediatR.Users.Logout;
using Streetcode.BLL.MediatR.Users.RefreshToken;
using Streetcode.BLL.MediatR.Users.Register;
using Streetcode.WebApi.Service.Interfaces;

namespace Streetcode.WebApi.Controllers.Users;

[Route("api/auth")]
[ExcludeFromCodeCoverage]
public sealed class AuthController : BaseApiController
{
    private readonly IGoogleAuthService _googleAuthService;

    public AuthController(IGoogleAuthService googleAuthService)
    {
        _googleAuthService = googleAuthService;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] UserLoginDto loginRequest, CancellationToken cancellationToken = default)
    {
        return base.HandleResult(
            await base.Mediator.Send(new LoginUserCommand(loginRequest), cancellationToken)
        );
    }

    [HttpPost("google-login")]
    public async Task<IActionResult> GoogleLogin([FromBody] GoogleLoginRequest request)
    {
        try
        {
            var payload = await _googleAuthService.ValidateTokenAsync(request.IdToken);

            if (payload == null)
            {
                return Unauthorized("Invalid Google Token");
            }

            var loginDto = new GoogleLoginRequestDto
            {
                Email = payload.Email,
                Name = payload.GivenName,
                Surname = payload.FamilyName
            };

            return base.HandleResult(
                await base.Mediator.Send(new GoogleLoginCommand(loginDto))
            );
        }
        catch (Exception)
        {
            return Unauthorized("Invalid Google Token");
        }
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] UserRegisterDto registerRequest, CancellationToken cancellationToken = default)
    {
        return base.HandleResult(
            await base.Mediator.Send(new RegisterUserCommand(registerRequest), cancellationToken)
        );
    }

    [HttpPost("refresh-token")]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequestDto refreshTokenRequest, CancellationToken cancellationToken = default)
    {
        return base.HandleResult(
            await base.Mediator.Send(new RefreshTokenCommand(refreshTokenRequest), cancellationToken)
        );
    }

    [Authorize]
    [HttpPost("logout")]
    public async Task<IActionResult> Logout(CancellationToken cancellationToken = default)
    {
        string? user_id_claim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(user_id_claim) || !int.TryParse(user_id_claim, out int user_id))
        {
            return base.Unauthorized();
        }
        return base.HandleResult(
            await base.Mediator.Send(new LogoutUserCommand(user_id), cancellationToken)
        );
    }
}