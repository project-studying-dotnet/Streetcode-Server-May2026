using FluentValidation;
using Streetcode.BLL.Resources;
using Streetcode.BLL.DTO.Timeline;
using Streetcode.DAL.Persistence.Constants;

namespace Streetcode.BLL.Validators.Timeline;

public sealed class HistoricalContextDtoValidator : AbstractValidator<HistoricalContextDto>
{
    public HistoricalContextDtoValidator()
    {
        base.RuleLevelCascadeMode = CascadeMode.Stop;
        base.RuleFor(dto => dto.Id).GreaterThanOrEqualTo(0).WithMessage(
            string.Format(ErrorMessages.PropertyMustBeGreaterThanOrEqualToValue, nameof(HistoricalContextDto.Id), 0)
        );
        base.RuleFor(dto => dto.Title).NotEmpty().WithMessage(
            string.Format(ErrorMessages.StringPropertyIsRequired, nameof(HistoricalContextDto.Title))
        ).MaximumLength(HistoricalContextConstants.TitleMaxLength).WithMessage(
            string.Format(ErrorMessages.StringPropertyMustNotExceedCharacters, nameof(HistoricalContextDto.Title), HistoricalContextConstants.TitleMaxLength)
        );
    }
}