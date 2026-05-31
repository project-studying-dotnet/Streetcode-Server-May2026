using Streetcode.BLL.MediatR.Timeline.TimelineItem.GetByStreetcodeId;

namespace Streetcode.BLL.Validators.Timeline.TimelineItem.GetByStreetcodeId
{
    public class GetTimelineItemsByStreetcodeIdQueryValidator
        : PositiveStreetcodeIdValidator<GetTimelineItemsByStreetcodeIdQuery>
    {
    }
}