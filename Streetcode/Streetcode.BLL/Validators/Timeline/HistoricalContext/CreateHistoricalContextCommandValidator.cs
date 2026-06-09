using FluentValidation;
using Streetcode.BLL.MediatR.Timeline.HistoricalContext;

namespace Streetcode.BLL.Validators.Timeline.HistoricalContext;

public sealed class CreateHistoricalContextCommandValidator : AbstractValidator<CreateHistoricalContextCommand>
{
    public CreateHistoricalContextCommandValidator()
    {
        base.RuleLevelCascadeMode = CascadeMode.Stop;
        base.RuleFor(command => command.HistoricalContext).SetValidator(new HistoricalContextDtoValidator());
    }
}