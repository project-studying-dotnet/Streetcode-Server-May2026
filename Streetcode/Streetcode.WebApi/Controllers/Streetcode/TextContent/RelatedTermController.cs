using Streetcode.DAL.Enums;
using Microsoft.AspNetCore.Mvc;
using Streetcode.WebApi.Attributes;
using Streetcode.BLL.DTO.Streetcode.TextContent;
using Streetcode.BLL.MediatR.Streetcode.RelatedTerm.Create;
using Streetcode.BLL.MediatR.Streetcode.RelatedTerm.Delete;
using Streetcode.BLL.MediatR.Streetcode.RelatedTerm.Update;
using Streetcode.BLL.DTO.Streetcode.TextContent.RelatedTerm;
using Streetcode.BLL.MediatR.Streetcode.RelatedTerm.GetAllByTermId;

namespace Streetcode.WebApi.Controllers.Streetcode.TextContent;

public sealed class RelatedTermController : BaseApiController
{
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetByTermId([FromRoute] int id, CancellationToken cancellationToken = default)
    {
        return base.HandleResult(
            await base.Mediator.Send(new GetAllRelatedTermsByTermIdQuery(id), cancellationToken)
        );
    }

    [HttpPost]
    [AuthorizeRoles(UserRole.MainAdministrator)]
    public async Task<IActionResult> Create([FromBody] CreateRelatedTermDto request, CancellationToken cancellationToken = default)
    {
        return base.HandleResult(
            await base.Mediator.Send(new CreateRelatedTermCommand(request), cancellationToken)
        );
    }

    [HttpPut("{id:int}")]
    [AuthorizeRoles(UserRole.MainAdministrator)]
    public async Task<IActionResult> Update([FromRoute] int id, [FromBody] RelatedTermDTO relatedTerm, CancellationToken cancellationToken = default)
    {
        return base.HandleResult(
            await base.Mediator.Send(new UpdateRelatedTermCommand(id, relatedTerm), cancellationToken)
        );
    }

    [HttpDelete("{word}/{termId:int}")]
    [AuthorizeRoles(UserRole.MainAdministrator)]
    public async Task<IActionResult> Delete([FromRoute] string word, [FromRoute] int termId, CancellationToken cancellationToken = default)
    {
        return base.HandleResult(
            await base.Mediator.Send(new DeleteRelatedTermCommand(word, termId), cancellationToken)
        );
    }
}