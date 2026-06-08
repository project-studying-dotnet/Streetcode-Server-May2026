using FluentValidation;
using Streetcode.BLL.MediatR.Timeline.HistoricalContext;

namespace Streetcode.BLL.Validators.Timeline.HistoricalContext;

public sealed class UpdateHistoricalContextCommandValidator : AbstractValidator<UpdateHistoricalContextCommand>
{
    public UpdateHistoricalContextCommandValidator()
    {
        base.RuleLevelCascadeMode = CascadeMode.Stop;
        base.RuleFor(command => command.HistoricalContext).SetValidator(new HistoricalContextDtoValidator());
    }
}