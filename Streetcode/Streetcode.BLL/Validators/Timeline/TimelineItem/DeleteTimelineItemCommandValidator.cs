using FluentValidation;
using Streetcode.BLL.Resources;
using Streetcode.BLL.MediatR.Timeline.TimelineItem;

namespace Streetcode.BLL.Validators.Timeline.TimelineItem;

public sealed class DeleteTimelineItemCommandValidator : AbstractValidator<DeleteTimelineItemCommand>
{
    public DeleteTimelineItemCommandValidator()
    {
        base.RuleLevelCascadeMode = CascadeMode.Stop;
        base.RuleFor(command => command.Id).GreaterThanOrEqualTo(0).WithMessage(
            string.Format(ErrorMessages.PropertyMustBeGreaterThanOrEqualToValue, nameof(DeleteTimelineItemCommand.Id), 0)
        );
    }
}