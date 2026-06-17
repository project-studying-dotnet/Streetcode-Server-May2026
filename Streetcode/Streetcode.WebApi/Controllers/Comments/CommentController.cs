using System.Diagnostics.CodeAnalysis;
using Streetcode.DAL.Enums;
using Microsoft.AspNetCore.Mvc;
using Streetcode.BLL.DTO.Comments;
using Streetcode.WebApi.Attributes;
using Streetcode.BLL.MediatR.Comments.Create;
using Streetcode.BLL.MediatR.Comments.Delete;
using Streetcode.BLL.MediatR.Comments.GetByStreetcodeId;
using Streetcode.BLL.MediatR.Comments.GetRepliesByCommentId;
using Streetcode.BLL.MediatR.Comments.Update;

namespace Streetcode.WebApi.Controllers.Comments;

[ExcludeFromCodeCoverage]
public sealed class CommentController : BaseApiController
{
    [HttpGet("{streetcodeId:int}")]
    public async Task<IActionResult> GetByStreetcodeId([FromRoute] int streetcodeId, CancellationToken cancellationToken = default)
    {
        return base.HandleResult(
            await base.Mediator.Send(new GetCommentsByStreetcodeIdQuery(streetcodeId), cancellationToken)
        );
    }

    [HttpGet("{commentId:int}")]
    public async Task<IActionResult> GetRepliesByCommentId([FromRoute] int commentId, CancellationToken cancellationToken = default)
    {
        return base.HandleResult(
            await base.Mediator.Send(new GetRepliesByCommentIdQuery(commentId), cancellationToken)
        );
    }

    [HttpPost]
    [AuthorizeRoles([UserRole.Administrator, UserRole.MainAdministrator])]
    public async Task<IActionResult> Create([FromBody] CreateCommentDto request, CancellationToken cancellationToken = default)
    {
        return base.HandleResult(
            await base.Mediator.Send(new CreateCommentCommand(request), cancellationToken)
        );
    }

    [HttpPut]
    [AuthorizeRoles([UserRole.Administrator, UserRole.MainAdministrator])]
    public async Task<IActionResult> Update([FromBody] UpdateCommentDto request, CancellationToken cancellationToken = default)
    {
        return base.HandleResult(
            await base.Mediator.Send(new UpdateCommentCommand(request), cancellationToken)
        );
    }

    [HttpDelete("{commentId:int}")]
    [AuthorizeRoles([UserRole.Administrator, UserRole.MainAdministrator])]
    public async Task<IActionResult> Delete([FromRoute] int commentId, CancellationToken cancellationToken = default)
    {
        return base.HandleResult(
            await base.Mediator.Send(new DeleteCommentCommand(commentId), cancellationToken)
        );
    }
}