using Streetcode.BLL.MediatR.Timeline.TimelineItem.GetByStreetcodeId;

namespace Streetcode.BLL.Validators.Timeline.TimelineItem.GetByStreetcodeId
{
    /// <summary>
    /// Validator for <see cref="GetTimelineItemsByStreetcodeIdQuery"/>.
    /// </summary>
    public class GetTimelineItemsByStreetcodeIdQueryValidator
        : PositiveStreetcodeIdValidator<GetTimelineItemsByStreetcodeIdQuery>
    {
    }
}