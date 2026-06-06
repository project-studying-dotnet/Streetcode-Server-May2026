using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Streetcode.Common.Enums;

namespace Streetcode.Auth.Controllers;

[ApiController]
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
            return (result.Value == null) ? NotFound() : Ok(result.Value);
        }

        var error = result.Errors.FirstOrDefault();

        return error switch
        {
            { } => BadRequest(error.Message),
            _ => BadRequest("Something went wrong")
        };
    }
}