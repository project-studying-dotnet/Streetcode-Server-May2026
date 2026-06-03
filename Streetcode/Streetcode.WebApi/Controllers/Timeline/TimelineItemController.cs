using Microsoft.AspNetCore.Mvc;
using Streetcode.BLL.DTO.Timeline;
using Streetcode.BLL.MediatR.Timeline.TimelineItem.Create;
using Streetcode.BLL.MediatR.Timeline.TimelineItem.Update;
using Streetcode.BLL.MediatR.Timeline.TimelineItem.Delete;
using Streetcode.BLL.MediatR.Timeline.TimelineItem.GetAll;
using Streetcode.BLL.MediatR.Timeline.TimelineItem.GetById;
using Streetcode.BLL.MediatR.Timeline.TimelineItem.GetByStreetcodeId;
using Streetcode.DAL.Enums;
using Streetcode.WebApi.Attributes;

namespace Streetcode.WebApi.Controllers.Timeline;

public sealed class TimelineItemController : BaseApiController
{
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        return HandleResult(await Mediator.Send(new GetAllTimelineItemsQuery()));
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById([FromRoute] int id)
    {
        return HandleResult(await Mediator.Send(new GetTimelineItemByIdQuery(id)));
    }

    [HttpGet("{streetcodeId:int}")]
    public async Task<IActionResult> GetByStreetcodeId([FromRoute] int streetcodeId)
    {
        return HandleResult(await Mediator.Send(new GetTimelineItemsByStreetcodeIdQuery(streetcodeId)));
    }

    [AuthorizeRoles(UserRole.MainAdministrator)]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] TimelineItemDto timelineItem, CancellationToken cancellationToken)
    {
        return HandleResult(
            await Mediator.Send(new CreateTimelineItemCommand(timelineItem), cancellationToken)
        );
    }

    [AuthorizeRoles(UserRole.MainAdministrator)]
    [HttpPut]
    public async Task<IActionResult> Update([FromBody] TimelineItemDto timelineItem, CancellationToken cancellationToken)
    {
        return HandleResult(
            await Mediator.Send(new UpdateTimelineItemCommand(timelineItem), cancellationToken)
        );
    }

    [AuthorizeRoles(UserRole.MainAdministrator)]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete([FromRoute] int id, CancellationToken cancellationToken)
    {
        return HandleResult(
            await Mediator.Send(new DeleteTimelineItemCommand(id), cancellationToken)
        );
    }
}