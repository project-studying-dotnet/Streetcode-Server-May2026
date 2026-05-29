using FluentValidation;
using Streetcode.BLL.DTO.Partners;
using Streetcode.BLL.Resources;

namespace Streetcode.BLL.Validators.Partners.Create
{
    public class CreatePartnerDtoValidator : AbstractValidator<CreatePartnerDTO>
    {
        private const int MaxDescriptionLength = 400;
        private const int MaxUrlTitleLength = 255;

        public CreatePartnerDtoValidator()
        {
            RuleLevelCascadeMode = CascadeMode.Stop;

            RuleFor(x => x.Title)
                .NotEmpty()
                .WithMessage(ErrorMessages.TitleIsRequired);

            RuleFor(x => x.Description)
                .MaximumLength(MaxDescriptionLength)
                .WithMessage(string.Format(ErrorMessages.DescriptionMustNotExceedCharacters, MaxDescriptionLength))
                .When(x => !string.IsNullOrWhiteSpace(x.Description));

            RuleFor(x => x.TargetUrl)
                .Must(BeValidUrl)
                .WithMessage(ErrorMessages.TargetUrlMustBeValid)
                .When(x => !string.IsNullOrWhiteSpace(x.TargetUrl));

            RuleFor(x => x.UrlTitle)
                .MaximumLength(MaxUrlTitleLength)
                .WithMessage(string.Format(ErrorMessages.UrlTitleMustNotExceedCharacters, MaxUrlTitleLength))
                .When(x => !string.IsNullOrWhiteSpace(x.UrlTitle));

            RuleFor(x => x.LogoId)
                .GreaterThan(0)
                .WithMessage(ErrorMessages.LogoIdMustBePositive);

            RuleForEach(x => x.PartnerSourceLinks)
                .SetValidator(new CreatePartnerSourceLinkDtoValidator());

            RuleForEach(x => x.Streetcodes)
                .SetValidator(new StreetcodeShortDtoValidator());
        }

        private static bool BeValidUrl(string? url)
        {
            return Uri.TryCreate(url, UriKind.Absolute, out _);
        }
    }
}