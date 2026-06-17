using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Streetcode.BLL.DTO.Users;
using Streetcode.BLL.MediatR.Users.ForgotPassword;
using Streetcode.BLL.MediatR.Users.Login;
using Streetcode.BLL.MediatR.Users.Logout;
using Streetcode.BLL.MediatR.Users.RefreshToken;
using Streetcode.BLL.MediatR.Users.Register;
using Streetcode.BLL.MediatR.Users.ResetPassword;
using System.Diagnostics.CodeAnalysis;
using System.Security.Claims;

namespace Streetcode.WebApi.Controllers.Users;

[Route("api/auth")]
[ExcludeFromCodeCoverage]
public sealed class AuthController : BaseApiController
{
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] UserLoginDto loginRequest, CancellationToken cancellationToken = default)
    {
        return base.HandleResult(
            await base.Mediator.Send(new LoginUserCommand(loginRequest), cancellationToken)
        );
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
        if(string.IsNullOrEmpty(user_id_claim) || !int.TryParse(user_id_claim, out int user_id))
        {
            return base.Unauthorized();
        }
        return base.HandleResult(
            await base.Mediator.Send(new LogoutUserCommand(user_id), cancellationToken)
        );
    }

    [HttpPost("forgot-password")]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto forgotPasswordDto)
    {
        return HandleResult(await Mediator.Send(new ForgotPasswordCommand(forgotPasswordDto)));
    }

    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto resetPasswordDto)
    {
        return HandleResult(await Mediator.Send(new ResetPasswordCommand(resetPasswordDto)));
    }
}