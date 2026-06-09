using FluentValidation;
using Streetcode.DAL.Enums;
using Streetcode.BLL.Resources;
using Streetcode.BLL.DTO.Timeline;
using Streetcode.DAL.Persistence.Constants;

namespace Streetcode.BLL.Validators.Timeline;

public sealed class TimelineItemDtoValidator : AbstractValidator<TimelineItemDto>
{
    public TimelineItemDtoValidator()
    {
        base.RuleLevelCascadeMode = CascadeMode.Stop;
        base.RuleFor(dto => dto.Id).GreaterThanOrEqualTo(0).WithMessage(
            string.Format(ErrorMessages.PropertyMustBeGreaterThanOrEqualToValue, nameof(TimelineItemDto.Id), 0)
        );
        base.RuleFor(dto => dto.Title).NotEmpty().WithMessage(
            string.Format(ErrorMessages.StringPropertyIsRequired, nameof(TimelineItemDto.Title))
        ).MaximumLength(TimelineItemConstants.TitleMaxLength).WithMessage(
            string.Format(ErrorMessages.StringPropertyMustNotExceedCharacters, nameof(TimelineItemDto.Title), TimelineItemConstants.TitleMaxLength)
        );
        base.RuleFor(dto => dto.Description).MaximumLength(TimelineItemConstants.DescriptionMaxLength).WithMessage(
            string.Format(ErrorMessages.StringPropertyMustNotExceedCharacters, nameof(TimelineItemDto.Description), TimelineItemConstants.DescriptionMaxLength)
        ).When(dto => dto.Description is not null);
        base.RuleFor(dto => dto.Date).NotEmpty().WithMessage(
            string.Format(ErrorMessages.StructPropertyIsRequired, nameof(TimelineItemDto.Date))
        );
        base.RuleFor(dto => dto.DateViewPattern).IsInEnum().WithMessage(
            string.Format(ErrorMessages.PropertyMustBeValidEnumValue, nameof(TimelineItemDto.DateViewPattern), nameof(DateViewPattern))
        );
        base.RuleFor(dto => dto.StreetcodeId).GreaterThanOrEqualTo(0).WithMessage(
            string.Format(ErrorMessages.PropertyMustBeGreaterThanOrEqualToValue, nameof(TimelineItemDto.StreetcodeId), 0)
        ).When(dto => dto.StreetcodeId is not null);
    }
}