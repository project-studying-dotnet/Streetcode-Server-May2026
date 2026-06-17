using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Streetcode.Auth.Extensions;
using Streetcode.Auth.MediatR.Users.Login;
using Streetcode.Auth.MediatR.Users.Logout;
using Streetcode.Auth.MediatR.Users.RefreshToken;
using Streetcode.Auth.MediatR.Users.Register;
using Streetcode.Auth.Models.DTO;
using Streetcode.Auth.Models.DTO.Users;
using Streetcode.Auth.Models.MediatR.Users.ChangePassword;
using System.Security.Claims;

namespace Streetcode.Auth.Controllers.Users;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator;

    public AuthController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<IActionResult> Login(UserLoginDto request)
    {
        var result = await _mediator.Send(new LoginUserCommand(request));
        return this.ToActionResult(result);
    }

    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<IActionResult> Register(UserRegisterDto request)
    {
        var result = await _mediator.Send(new RegisterUserCommand(request));
        return this.ToActionResult(result);
    }

    [AllowAnonymous]
    [HttpPost("refresh-token")]
    public async Task<IActionResult> RefreshToken(RefreshTokenRequestDto request)
    {
        var result = await _mediator.Send(new RefreshTokenCommand(request));
        return this.ToActionResult(result);
    }

    [Authorize]
    [HttpPost("logout")]
    public async Task<IActionResult> Logout([FromHeader(Name = "X-Refresh-Token")] string refreshToken)
    {
        if (string.IsNullOrWhiteSpace(refreshToken))
            return BadRequest("Refresh token missing in X-Refresh-Token header.");

        var result = await _mediator.Send(new LogoutUserCommand(refreshToken));
        return this.ToActionResult(result);
    }

    [Authorize]
    [HttpPost("change-password")]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto request, CancellationToken cancellationToken = default)
    {
        string? userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
        {
            return Unauthorized();
        }

        var result = await _mediator.Send(new ChangePasswordCommand(userId, request), cancellationToken);
        return this.ToActionResult(result);
    }
}