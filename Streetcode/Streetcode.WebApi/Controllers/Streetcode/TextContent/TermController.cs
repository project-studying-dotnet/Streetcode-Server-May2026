using System.Diagnostics.CodeAnalysis;
using Streetcode.DAL.Enums;
using Microsoft.AspNetCore.Mvc;
using Streetcode.WebApi.Attributes;
using Streetcode.BLL.MediatR.Streetcode.Term.Create;
using Streetcode.BLL.MediatR.Streetcode.Term.Delete;
using Streetcode.BLL.MediatR.Streetcode.Term.GetAll;
using Streetcode.BLL.MediatR.Streetcode.Term.Update;
using Streetcode.BLL.MediatR.Streetcode.Term.GetById;
using Streetcode.BLL.DTO.Streetcode.TextContent.Term;

namespace Streetcode.WebApi.Controllers.Streetcode.TextContent;

[ExcludeFromCodeCoverage]
public sealed class TermController : BaseApiController
{
    [HttpPost]
    [AuthorizeRoles(UserRole.MainAdministrator)]
    public async Task<IActionResult> Create([FromBody] CreateTermDto term, CancellationToken cancellationToken = default)
    {
        return base.HandleResult(
            await base.Mediator.Send(new CreateTermCommand(term), cancellationToken)
        );
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken = default)
    {
        return base.HandleResult(
            await base.Mediator.Send(new GetAllTermsQuery(), cancellationToken)
        );
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById([FromRoute] int id, CancellationToken cancellationToken = default)
    {
        return base.HandleResult(
            await base.Mediator.Send(new GetTermByIdQuery(id), cancellationToken)
        );
    }

    [HttpPut]
    [AuthorizeRoles(UserRole.MainAdministrator)]
    public async Task<IActionResult> Update([FromBody] UpdateTermDto term, CancellationToken cancellationToken = default)
    {
        return base.HandleResult(
            await base.Mediator.Send(new UpdateTermCommand(term), cancellationToken)
        );
    }

    [HttpDelete("{id:int}")]
    [AuthorizeRoles(UserRole.MainAdministrator)]
    public async Task<IActionResult> Delete([FromRoute] int id, CancellationToken cancellationToken = default)
    {
        return base.HandleResult(
            await base.Mediator.Send(new DeleteTermCommand(id), cancellationToken)
        );
    }
}