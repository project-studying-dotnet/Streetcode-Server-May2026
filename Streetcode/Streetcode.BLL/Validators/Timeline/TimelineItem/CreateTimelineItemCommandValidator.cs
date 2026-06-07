using FluentValidation;
using Streetcode.BLL.MediatR.Timeline.TimelineItem;

namespace Streetcode.BLL.Validators.Timeline.TimelineItem;

public sealed class CreateTimelineItemCommandValidator : AbstractValidator<CreateTimelineItemCommand>
{
    public CreateTimelineItemCommandValidator()
    {
        base.RuleLevelCascadeMode = CascadeMode.Stop;
        base.RuleFor(command => command.TimelineItem).SetValidator(new TimelineItemDtoValidator());
    }
}