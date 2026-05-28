using FluentValidation;
using Streetcode.BLL.DTO.Timeline;
using Streetcode.BLL.Resources;

namespace Streetcode.BLL.Validators.Timeline.TimelineItem
{
    /// <summary>
    /// Validator for <see cref="TimelineItemDto"/>.
    /// </summary>
    public class TimelineItemDtoValidator : AbstractValidator<TimelineItemDto>
    {
        private const int MaxTitleLength = 26;
        private const int MaxDescriptionLength = 400;

        /// <summary>
        /// Initializes a new instance of the <see cref="TimelineItemDtoValidator"/> class.
        /// </summary>
        public TimelineItemDtoValidator()
        {
            RuleLevelCascadeMode = CascadeMode.Stop;

            RuleFor(x => x.Id)
                .GreaterThanOrEqualTo(0)
                .WithMessage(ErrorMessages.IdMustBeGreaterOrEqualToZero);

            RuleFor(x => x.Title)
                .NotEmpty()
                .WithMessage(ErrorMessages.TitleIsRequired)
                .MaximumLength(MaxTitleLength)
                .WithMessage(string.Format(ErrorMessages.TitleMustNotExceedCharacters, MaxTitleLength));

            RuleFor(x => x.Description)
                .MaximumLength(MaxDescriptionLength)
                .WithMessage(string.Format(ErrorMessages.DescriptionMustNotExceedCharacters, MaxDescriptionLength))
                .When(x => !string.IsNullOrWhiteSpace(x.Description));

            RuleFor(x => x.Date)
                .NotEmpty()
                .WithMessage(ErrorMessages.DateIsRequired);

            RuleFor(x => x.DateViewPattern)
                .IsInEnum()
                .WithMessage(ErrorMessages.InvalidDateViewPattern);

            RuleFor(x => x.HistoricalContexts)
                .NotNull()
                .WithMessage(ErrorMessages.HistoricalContextsCollectionIsRequired);
        }
    }
}