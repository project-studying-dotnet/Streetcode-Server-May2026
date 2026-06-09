using Streetcode.DAL.Enums;
using Microsoft.AspNetCore.Mvc;
using Streetcode.BLL.DTO.Comments;
using Streetcode.WebApi.Attributes;
using Streetcode.BLL.MediatR.Comments.Create;

namespace Streetcode.WebApi.Controllers.Comments;

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
}