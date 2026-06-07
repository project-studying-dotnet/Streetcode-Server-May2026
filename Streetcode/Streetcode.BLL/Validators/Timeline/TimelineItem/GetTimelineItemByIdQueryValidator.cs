using FluentValidation;
using Streetcode.BLL.Resources;
using Streetcode.BLL.MediatR.Timeline.TimelineItem;

namespace Streetcode.BLL.Validators.Timeline.TimelineItem;

public sealed class GetTimelineItemByIdQueryValidator : AbstractValidator<GetTimelineItemByIdQuery>
{
    public GetTimelineItemByIdQueryValidator()
    {
        base.RuleLevelCascadeMode = CascadeMode.Stop;
        base.RuleFor(query => query.Id).GreaterThanOrEqualTo(0).WithMessage(
            string.Format(ErrorMessages.PropertyMustBeGreaterThanOrEqualToValue, nameof(GetTimelineItemByIdQuery.Id), 0)
        );
    }
}