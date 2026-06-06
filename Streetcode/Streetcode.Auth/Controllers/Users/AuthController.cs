using System.Diagnostics.CodeAnalysis;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Streetcode.Auth.Models.DTO;
using Streetcode.Auth.MediatR.Users.Login;
using Streetcode.Auth.MediatR.Users.Logout;
using Streetcode.Auth.MediatR.Users.RefreshToken;
using Streetcode.Auth.MediatR.Users.Register;

namespace Streetcode.Auth.Controllers.Users
{
    [ExcludeFromCodeCoverage]

    [Route("api/auth")]
    public class AuthController : BaseApiController
    {
        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserLoginDto loginRequest)
        {
            try
            {
                return HandleResult(await Mediator.Send(new LoginUserCommand(loginRequest)));
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserRegisterDto registerRequest)
        {
            try
            {
                return HandleResult(await Mediator.Send(new RegisterUserCommand(registerRequest)));
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
         }

        [AllowAnonymous]
        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequestDto refreshTokenRequest)
        {
            return HandleResult(await Mediator.Send(new RefreshTokenCommand(refreshTokenRequest)));
        }

        [Authorize]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout([FromBody] LogoutRequestDto request)
        {
            if (string.IsNullOrEmpty(request.RefreshToken))
            {
                return BadRequest("Refresh token is required.");
            }
            return HandleResult(await Mediator.Send(new LogoutUserCommand(request.RefreshToken)));
        }
    }
}