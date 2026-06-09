using System.Collections.Frozen;
using MediatR;
using FluentResults;
using Streetcode.DAL.Enums;
using Microsoft.AspNetCore.Mvc;
using Streetcode.BLL.Resources;
using Streetcode.BLL.MediatR.ResultVariations;

namespace Streetcode.WebApi.Controllers;

[ApiController]
[Route("api/[controller]/[action]")]
public class BaseApiController : ControllerBase
{
    #region Static
    private static FrozenSet<UserRole> UserRoles { get; set; } = Enum.GetValues<UserRole>().ToFrozenSet();
    #endregion

    #region Instance
    protected IMediator Mediator
    {
        get => field ??= base.HttpContext.RequestServices.GetRequiredService<IMediator>();
    }

    protected UserRole? GetUserRole()
    {
        foreach(UserRole role in BaseApiController.UserRoles)
        {
            string role_str = role.ToString();
            if(base.User.IsInRole(role_str))
            {
                return role;
            }
        }
        return null;
    }
    protected ActionResult HandleResult<T>(Result<T> result)
    {
        if(result.IsSuccess)
        {
            if(result is NullResult<T>)
            {
                return base.Ok(result.Value);
            }
            return (result.Value is null) ? base.NotFound(ErrorMessages.FoundResultMatchingNull) : base.Ok(result.Value);
        }
        return base.BadRequest(result.Reasons);
    }
    #endregion
}