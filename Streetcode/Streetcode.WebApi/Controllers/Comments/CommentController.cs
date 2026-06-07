using Microsoft.AspNetCore.Mvc;
using Streetcode.BLL.DTO.Comments;
using Streetcode.BLL.MediatR.Comments.Create;

namespace Streetcode.WebApi.Controllers.Comments;

public class CommentController : BaseApiController
{
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateCommentDto request, CancellationToken cancellationToken = default)
    {
        return HandleResult(await Mediator.Send(new CreateCommentCommand(request), cancellationToken));
    }
}
