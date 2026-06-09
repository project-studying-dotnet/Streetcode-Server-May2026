using Streetcode.DAL.Enums;
using Microsoft.AspNetCore.Mvc;
using Streetcode.WebApi.Attributes;
using Streetcode.BLL.MediatR.Streetcode.Fact.Create;
using Streetcode.BLL.MediatR.Streetcode.Fact.Delete;
using Streetcode.BLL.MediatR.Streetcode.Fact.GetAll;
using Streetcode.BLL.MediatR.Streetcode.Fact.Update;
using Streetcode.BLL.MediatR.Streetcode.Fact.GetById;
using Streetcode.BLL.DTO.Streetcode.TextContent.Fact;
using Streetcode.BLL.MediatR.Streetcode.Fact.GetByStreetcodeId;

namespace Streetcode.WebApi.Controllers.Streetcode.TextContent;

public sealed class FactController : BaseApiController
{
    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken = default)
    {
        return base.HandleResult(
            await base.Mediator.Send(new GetAllFactsQuery(), cancellationToken)
        );
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById([FromRoute] int id, CancellationToken cancellationToken = default)
    {
        return base.HandleResult(
            await base.Mediator.Send(new GetFactByIdQuery(id), cancellationToken)
        );
    }

    [HttpGet("{streetcodeId:int}")]
    public async Task<IActionResult> GetByStreetcodeId([FromRoute] int streetcodeId, CancellationToken cancellationToken = default)
    {
        return base.HandleResult(
            await base.Mediator.Send(new GetFactByStreetcodeIdQuery(streetcodeId), cancellationToken)
        );
    }

    [HttpPost]
    [AuthorizeRoles(UserRole.MainAdministrator)]
    public async Task<IActionResult> Create([FromBody] FactDto request, CancellationToken cancellationToken = default)
    {
        return base.HandleResult(
            await base.Mediator.Send(new CreateFactCommand(request), cancellationToken)
        );
    }

    [HttpPut]
    [AuthorizeRoles(UserRole.MainAdministrator)]
    public async Task<IActionResult> Update([FromBody] FactDto fact, CancellationToken cancellationToken = default)
    {
        return base.HandleResult(
            await base.Mediator.Send(new UpdateFactCommand(fact), cancellationToken)
        );
    }

    [HttpDelete("{id:int}")]
    [AuthorizeRoles(UserRole.MainAdministrator)]
    public async Task<IActionResult> Delete([FromRoute] int id, CancellationToken cancellationToken = default)
    {
        return base.HandleResult(
            await base.Mediator.Send(new DeleteFactCommand(id), cancellationToken)
        );
    }
}