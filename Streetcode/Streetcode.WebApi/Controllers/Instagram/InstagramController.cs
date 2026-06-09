using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Mvc;
using Streetcode.BLL.MediatR.Instagram.GetAll;

namespace Streetcode.WebApi.Controllers.Instagram;

[ExcludeFromCodeCoverage]
public sealed class InstagramController : BaseApiController
{
    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken = default)
    {
        return base.HandleResult(
            await base.Mediator.Send(new GetAllPostsQuery(), cancellationToken)
        );
    }
}