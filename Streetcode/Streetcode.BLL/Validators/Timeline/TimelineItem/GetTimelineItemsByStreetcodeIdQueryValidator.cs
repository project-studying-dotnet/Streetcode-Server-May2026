using FluentValidation;
using Streetcode.BLL.Resources;
using Streetcode.BLL.MediatR.Timeline.TimelineItem;

namespace Streetcode.BLL.Validators.Timeline.TimelineItem;

public sealed class GetTimelineItemsByStreetcodeIdQueryValidator : AbstractValidator<GetTimelineItemsByStreetcodeIdQuery>
{
    public GetTimelineItemsByStreetcodeIdQueryValidator()
    {
        base.RuleLevelCascadeMode = CascadeMode.Stop;
        base.RuleFor(query => query.StreetcodeId).GreaterThanOrEqualTo(0).WithMessage(
            string.Format(ErrorMessages.PropertyMustBeGreaterThanOrEqualToValue, nameof(GetTimelineItemsByStreetcodeIdQuery.StreetcodeId), 0)
        );
    }
}