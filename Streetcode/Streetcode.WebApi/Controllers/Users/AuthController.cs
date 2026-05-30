using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Streetcode.BLL.DTO.Users;
using Streetcode.BLL.MediatR.Users.Login;

namespace Streetcode.WebApi.Controllers.Users
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
    }
}