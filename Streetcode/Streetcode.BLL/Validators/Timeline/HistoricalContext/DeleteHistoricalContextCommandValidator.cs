using FluentValidation;
using Streetcode.BLL.Resources;
using Streetcode.BLL.MediatR.Timeline.HistoricalContext;

namespace Streetcode.BLL.Validators.Timeline.HistoricalContext;

public sealed class DeleteHistoricalContextCommandValidator : AbstractValidator<DeleteHistoricalContextCommand>
{
    public DeleteHistoricalContextCommandValidator()
    {
        base.RuleLevelCascadeMode = CascadeMode.Stop;
        base.RuleFor(command => command.Id).GreaterThanOrEqualTo(0).WithMessage(
            string.Format(ErrorMessages.PropertyMustBeGreaterThanOrEqualToValue, nameof(DeleteHistoricalContextCommand.Id), 0)
        );
    }
}