using FluentValidation;
using Streetcode.BLL.DTO.Streetcode;

namespace Streetcode.BLL.MediatR.Streetcode.Streetcode
{
    /// <summary>
    /// Validator for <see cref="StreetcodeDTO"/>.
    /// </summary>
    public class StreetcodeDtoValidator : AbstractValidator<StreetcodeDTO>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StreetcodeDtoValidator"/> class.
        /// </summary>
        public StreetcodeDtoValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThanOrEqualTo(0)
                .WithMessage("Id must be greater than or equal to 0");

            RuleFor(x => x.Index)
                .GreaterThanOrEqualTo(0)
                .WithMessage("Index must be greater than or equal to 0");

            RuleFor(x => x.Title)
                .NotEmpty()
                .WithMessage("Title is required");

            RuleFor(x => x.DateString)
                .NotEmpty()
                .WithMessage("Date string is required");

            RuleFor(x => x.Alias)
                .MaximumLength(255)
                .WithMessage("Alias must not exceed 255 characters")
                .When(x => !string.IsNullOrWhiteSpace(x.Alias));

            RuleFor(x => x.TransliterationUrl)
                .NotEmpty()
                .WithMessage("Transliteration URL is required");

            RuleFor(x => x.Status)
                .IsInEnum()
                .WithMessage("Invalid streetcode status");

            RuleFor(x => x.EventStartOrPersonBirthDate)
                .NotEmpty()
                .WithMessage("Start or birth date is required");

            RuleFor(x => x.EventEndOrPersonDeathDate)
                .GreaterThanOrEqualTo(x => x.EventStartOrPersonBirthDate)
                .WithMessage("End or death date cannot be earlier than start or birth date")
                .When(x => x.EventEndOrPersonDeathDate.HasValue);

            RuleFor(x => x.ViewCount)
                .GreaterThanOrEqualTo(0)
                .WithMessage("View count cannot be negative");

            RuleFor(x => x.Tags)
                .NotNull()
                .WithMessage("Tags collection is required");

            RuleFor(x => x.Teaser)
                .NotEmpty()
                .WithMessage("Teaser is required")
                .Must(BeValidTeaserLength)
                .WithMessage("Teaser must contain up to 455 characters with paragraph breaks or up to 520 characters without paragraph breaks");

            RuleFor(x => x.StreetcodeType)
                .IsInEnum()
                .WithMessage("Invalid streetcode type");
        }

        private static bool BeValidTeaserLength(string teaser)
        {
            if (string.IsNullOrWhiteSpace(teaser))
            {
                return false;
            }

            bool hasParagraphBreak = teaser.Contains(Environment.NewLine) || teaser.Contains('\n');

            return hasParagraphBreak
                ? teaser.Length <= 455
                : teaser.Length <= 520;
        }
    }
}
