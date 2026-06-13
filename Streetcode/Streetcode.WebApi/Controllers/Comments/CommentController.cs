using System.Diagnostics.CodeAnalysis;
using Streetcode.DAL.Enums;
using Microsoft.AspNetCore.Mvc;
using Streetcode.BLL.DTO.Comments;
using Streetcode.WebApi.Attributes;
using Streetcode.BLL.MediatR.Comments.Create;
using Streetcode.BLL.MediatR.Comments.Update;

namespace Streetcode.WebApi.Controllers.Comments;

[ExcludeFromCodeCoverage]
public sealed class CommentController : BaseApiController
{
    [HttpPost]
    [AuthorizeRoles(UserRole.Administrator)]
    public async Task<IActionResult> Create([FromBody] CreateCommentDto request, CancellationToken cancellationToken = default)
    {
        return base.HandleResult(
            await base.Mediator.Send(new CreateCommentCommand(request), cancellationToken)
        );
    }

    [HttpPut]
    public async Task<IActionResult> Update([FromBody] UpdateCommentDto request, CancellationToken cancellationToken = default)
    {
        return HandleResult(await Mediator.Send(new UpdateCommentCommand(request), cancellationToken));
    }
}
