using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Mvc;
using Streetcode.BLL.DTO.Users;
using Streetcode.BLL.MediatR.Users.Login;
using Streetcode.BLL.MediatR.Users.Register;

namespace Streetcode.WebApi.Controllers.Users
{
    [ExcludeFromCodeCoverage]

    [Route("api/auth")]
    public class AuthController : BaseApiController
    {
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
    }
}