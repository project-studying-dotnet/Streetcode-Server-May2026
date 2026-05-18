using Streetcode.BLL.MediatR.Timeline.TimelineItem.GetById;

namespace Streetcode.BLL.Validators.Timeline.TimelineItem.GetById
{
    /// <summary>
    /// Validator for <see cref="GetTimelineItemByIdQuery"/>.
    /// </summary>
    public class GetTimelineItemByIdQueryValidator : PositiveIdValidator<GetTimelineItemByIdQuery>
    {
    }
}