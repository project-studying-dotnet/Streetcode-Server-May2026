using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Streetcode.BLL.MediatR.ResultVariations;
using Streetcode.DAL.Enums;

namespace Streetcode.WebApi.Controllers;

[ApiController]
[Route("api/[controller]/[action]")]
public class BaseApiController : ControllerBase
{
    private IMediator? _mediator;

    protected IMediator Mediator => _mediator ??=
        HttpContext.RequestServices.GetService<IMediator>() !;

    protected UserRole? GetUserRole()
    {
        foreach (UserRole role in Enum.GetValues<UserRole>())
        {
            if (User.IsInRole(role.ToString()))
            {
                return role;
            }
        }

        return null;
    }

    protected ActionResult HandleResult<T>(Result<T> result)
    {
        if (result.IsSuccess)
        {
            if(result is NullResult<T>)
            {
                return Ok(result.Value);
            }

            return (result.Value is null) ?
                NotFound("Found result matching null") : Ok(result.Value);
        }

        return BadRequest(result.Reasons);
    }
}