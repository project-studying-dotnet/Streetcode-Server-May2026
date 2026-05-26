using Microsoft.AspNetCore.Mvc;
using Streetcode.BLL.DTO.Timeline;
using Streetcode.BLL.MediatR.Timeline.HistoricalContext.Create;
using Streetcode.BLL.MediatR.Timeline.HistoricalContext.Delete;
using Streetcode.BLL.MediatR.Timeline.HistoricalContext.GetAll;
using Streetcode.BLL.MediatR.Timeline.HistoricalContext.Update;

namespace Streetcode.WebApi.Controllers.Timeline;

public sealed class HistoricalContextController : BaseApiController
{
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        return HandleResult(await Mediator.Send(new GetAllHistoricalContextQuery()));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] HistoricalContextDto dto)
    {
        return HandleResult(await Mediator.Send(new CreateHistoricalContextCommand(dto)));
    }

    [HttpPut]
    public async Task<IActionResult> Update([FromBody] HistoricalContextDto dto)
    {
        return HandleResult(await Mediator.Send(new UpdateHistoricalContextCommand(dto)));
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete([FromRoute] int id)
    {
        return HandleResult(await Mediator.Send(new DeleteHistoricalContextCommand(id)));
    }
}