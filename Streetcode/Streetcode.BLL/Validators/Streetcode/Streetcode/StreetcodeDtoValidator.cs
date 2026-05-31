using FluentValidation;
using Streetcode.BLL.DTO.Streetcode;
using Streetcode.BLL.Resources;

namespace Streetcode.BLL.Validators.Streetcode.Streetcode
{
    public class StreetcodeDtoValidator : AbstractValidator<StreetcodeDTO>
    {
        private const int MinIdValue = 0;
        private const int MinIndexValue = 0;
        private const int MinViewCount = 0;

        private const int MaxAliasLength = 255;
        private const int MaxTeaserLengthWithParagraphs = 455;
        private const int MaxTeaserLengthWithoutParagraphs = 520;
        public StreetcodeDtoValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThanOrEqualTo(MinIdValue)
                .WithMessage(string.Format(
                    ErrorMessages.ValueMustBeGreaterThanOrEqualTo,
                    nameof(StreetcodeDTO.Id),
                    MinIdValue));

            RuleFor(x => x.Index)
                    .GreaterThanOrEqualTo(MinIndexValue)
                    .WithMessage(string.Format(
                        ErrorMessages.ValueMustBeGreaterThanOrEqualTo,
                        nameof(StreetcodeDTO.Index),
                        MinIndexValue));

            RuleFor(x => x.Title)
                .NotEmpty()
                .WithMessage(string.Format(
                    ErrorMessages.FieldIsRequired,
                    nameof(StreetcodeDTO.Title)));

            RuleFor(x => x.DateString)
                .NotEmpty()
                .WithMessage(string.Format(
                    ErrorMessages.FieldIsRequired,
                    nameof(StreetcodeDTO.DateString)));

            RuleFor(x => x.Alias)
                .MaximumLength(MaxAliasLength)
                .WithMessage(string.Format(
                    ErrorMessages.FieldMustNotExceedCharacters,
                    nameof(StreetcodeDTO.Alias),
                    MaxAliasLength))
                .When(x => !string.IsNullOrWhiteSpace(x.Alias));

            RuleFor(x => x.TransliterationUrl)
                .NotEmpty()
                .WithMessage(string.Format(
                    ErrorMessages.FieldIsRequired,
                    nameof(StreetcodeDTO.TransliterationUrl)));

            RuleFor(x => x.Status)
               .IsInEnum()
               .WithMessage(string.Format(
                   ErrorMessages.InvalidEnumValue,
                   nameof(StreetcodeDTO.Status)));

            RuleFor(x => x.EventStartOrPersonBirthDate)
                .NotEmpty()
                .WithMessage(string.Format(
                    ErrorMessages.FieldIsRequired,
                    nameof(StreetcodeDTO.EventStartOrPersonBirthDate)));

            RuleFor(x => x.EventEndOrPersonDeathDate)
                .GreaterThanOrEqualTo(x => x.EventStartOrPersonBirthDate)
                .WithMessage(ErrorMessages.EndDateCannotBeEarlierThanStartDate)
                .When(x => x.EventEndOrPersonDeathDate.HasValue);

            RuleFor(x => x.ViewCount)
                .GreaterThanOrEqualTo(MinViewCount)
                .WithMessage(string.Format(
                    ErrorMessages.ValueCannotBeNegative,
                    nameof(StreetcodeDTO.ViewCount)));

            RuleFor(x => x.Tags)
                .NotNull()
                .WithMessage(string.Format(
                    ErrorMessages.CollectionIsRequired,
                    nameof(StreetcodeDTO.Tags)));

            RuleFor(x => x.Teaser)
                .NotEmpty()
                .WithMessage(string.Format(
                    ErrorMessages.FieldIsRequired,
                    nameof(StreetcodeDTO.Teaser)))
                .Must(BeValidTeaserLength)
                .WithMessage(string.Format(
                    ErrorMessages.TeaserLengthIsInvalid,
                    MaxTeaserLengthWithParagraphs,
                    MaxTeaserLengthWithoutParagraphs));

            RuleFor(x => x.StreetcodeType)
                .IsInEnum()
                .WithMessage(string.Format(
                    ErrorMessages.InvalidEnumValue,
                    nameof(StreetcodeDTO.StreetcodeType)));
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
