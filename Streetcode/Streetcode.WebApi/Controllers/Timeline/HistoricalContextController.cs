using Streetcode.DAL.Enums;
using Microsoft.AspNetCore.Mvc;
using Streetcode.BLL.DTO.Timeline;
using Streetcode.WebApi.Attributes;
using Streetcode.BLL.MediatR.Timeline.HistoricalContext;

namespace Streetcode.WebApi.Controllers.Timeline;

public sealed class HistoricalContextController : BaseApiController
{
    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        return base.HandleResult(
            await base.Mediator.Send(new GetAllHistoricalContextQuery(), cancellationToken)
        );
    }

    [HttpPost]
    [AuthorizeRoles(UserRole.MainAdministrator)]
    public async Task<IActionResult> Create([FromBody] HistoricalContextDto dto, CancellationToken cancellationToken)
    {
        return base.HandleResult(
            await base.Mediator.Send(new CreateHistoricalContextCommand(dto), cancellationToken)
        );
    }

    [HttpPut]
    [AuthorizeRoles(UserRole.MainAdministrator)]
    public async Task<IActionResult> Update([FromBody] HistoricalContextDto dto, CancellationToken cancellationToken)
    {
        return base.HandleResult(
            await base.Mediator.Send(new UpdateHistoricalContextCommand(dto), cancellationToken)
        );
    }

    [HttpDelete("{id:int}")]
    [AuthorizeRoles(UserRole.MainAdministrator)]
    public async Task<IActionResult> Delete([FromRoute] int id, CancellationToken cancellationToken)
    {
        return base.HandleResult(
            await base.Mediator.Send(new DeleteHistoricalContextCommand(id), cancellationToken)
        );
    }
}