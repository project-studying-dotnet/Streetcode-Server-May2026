using Streetcode.DAL.Enums;
using Microsoft.AspNetCore.Mvc;
using Streetcode.BLL.DTO.Timeline;
using Streetcode.WebApi.Attributes;
using Streetcode.BLL.MediatR.Timeline.TimelineItem;

namespace Streetcode.WebApi.Controllers.Timeline;

public sealed class TimelineItemController : BaseApiController
{
    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        return base.HandleResult(
            await base.Mediator.Send(new GetAllTimelineItemsQuery(), cancellationToken)
        );
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById([FromRoute] int id, CancellationToken cancellationToken)
    {
        return base.HandleResult(
            await base.Mediator.Send(new GetTimelineItemByIdQuery(id), cancellationToken)
        );
    }

    [HttpGet("{streetcodeId:int}")]
    public async Task<IActionResult> GetByStreetcodeId([FromRoute] int streetcodeId, CancellationToken cancellationToken)
    {
        return base.HandleResult(
            await base.Mediator.Send(new GetTimelineItemsByStreetcodeIdQuery(streetcodeId), cancellationToken)
        );
    }

    [HttpPost]
    [AuthorizeRoles(UserRole.MainAdministrator)]
    public async Task<IActionResult> Create([FromBody] TimelineItemDto timelineItem, CancellationToken cancellationToken)
    {
        return base.HandleResult(
            await base.Mediator.Send(new CreateTimelineItemCommand(timelineItem), cancellationToken)
        );
    }

    [HttpPut]
    [AuthorizeRoles(UserRole.MainAdministrator)]
    public async Task<IActionResult> Update([FromBody] TimelineItemDto timelineItem, CancellationToken cancellationToken)
    {
        return base.HandleResult(
            await base.Mediator.Send(new UpdateTimelineItemCommand(timelineItem), cancellationToken)
        );
    }

    [HttpDelete("{id:int}")]
    [AuthorizeRoles(UserRole.MainAdministrator)]
    public async Task<IActionResult> Delete([FromRoute] int id, CancellationToken cancellationToken)
    {
        return base.HandleResult(
            await base.Mediator.Send(new DeleteTimelineItemCommand(id), cancellationToken)
        );
    }
}