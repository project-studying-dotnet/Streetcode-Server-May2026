using FluentValidation;
using Streetcode.BLL.DTO.Timeline;

namespace Streetcode.BLL.Validators.Timeline.TimelineItem
{
    /// <summary>
    /// Validator for <see cref="TimelineItemDto"/>.
    /// </summary>
    public class TimelineItemDtoValidator : AbstractValidator<TimelineItemDto>
    {
        private const int TitleMaxLength = 26;
        private const int DescriptionMaxLength = 400;

        /// <summary>
        /// Initializes a new instance of the <see cref="TimelineItemDtoValidator"/> class.
        /// </summary>
        public TimelineItemDtoValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThanOrEqualTo(0)
                .WithMessage("Id must be greater than or equal to 0");

            RuleFor(x => x.Title)
                .NotEmpty()
                .WithMessage("Title is required")
                .MaximumLength(TitleMaxLength)
                .WithMessage($"Title must not exceed {TitleMaxLength} characters");

            RuleFor(x => x.Description)
                .MaximumLength(DescriptionMaxLength)
                .WithMessage($"Description must not exceed {DescriptionMaxLength} characters")
                .When(x => !string.IsNullOrWhiteSpace(x.Description));

            RuleFor(x => x.Date)
                .NotEmpty()
                .WithMessage("Date is required");

            RuleFor(x => x.DateViewPattern)
                .IsInEnum()
                .WithMessage("Invalid date view pattern");

            RuleFor(x => x.HistoricalContexts)
                .NotNull()
                .WithMessage("Historical contexts collection is required");
        }
    }
}