using Microsoft.AspNetCore.Mvc;
using Streetcode.BLL.DTO.Timeline;
using Streetcode.BLL.MediatR.Timeline.HistoricalContext.Create;
using Streetcode.BLL.MediatR.Timeline.HistoricalContext.Delete;
using Streetcode.BLL.MediatR.Timeline.HistoricalContext.GetAll;
using Streetcode.BLL.MediatR.Timeline.HistoricalContext.Update;
using Streetcode.DAL.Enums;
using Streetcode.WebApi.Attributes;

namespace Streetcode.WebApi.Controllers.Timeline;

public sealed class HistoricalContextController : BaseApiController
{
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        return HandleResult(await Mediator.Send(new GetAllHistoricalContextQuery()));
    }

    [AuthorizeRoles(UserRole.MainAdministrator)]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] HistoricalContextDto dto, CancellationToken cancellationToken)
    {
        return HandleResult(
            await Mediator.Send(new CreateHistoricalContextCommand(dto), cancellationToken)
        );
    }

    [AuthorizeRoles(UserRole.MainAdministrator)]
    [HttpPut]
    public async Task<IActionResult> Update([FromBody] HistoricalContextDto dto, CancellationToken cancellationToken)
    {
        return HandleResult(
            await Mediator.Send(new UpdateHistoricalContextCommand(dto), cancellationToken)
        );
    }

    [AuthorizeRoles(UserRole.MainAdministrator)]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete([FromRoute] int id, CancellationToken cancellationToken)
    {
        return HandleResult(
            await Mediator.Send(new DeleteHistoricalContextCommand(id), cancellationToken)
        );
    }
}