using FluentValidation;
using Streetcode.BLL.DTO.Partners;

namespace Streetcode.BLL.Validators.Partners.Create
{
    /// <summary>
    /// Validator for CreatePartnerDTO.
    /// </summary>
    public class CreatePartnerDtoValidator : AbstractValidator<CreatePartnerDTO>
    {
        private const int DescriptionMaxLength = 400;

        /// <summary>
        /// Initializes a new instance of the <see cref="CreatePartnerDtoValidator"/> class.
        /// </summary>
        public CreatePartnerDtoValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty()
                .WithMessage("Title is required");

            RuleFor(x => x.Description)
               .MaximumLength(DescriptionMaxLength)
               .WithMessage($"Description must not exceed {DescriptionMaxLength} characters")
               .When(x => !string.IsNullOrWhiteSpace(x.Description));

            RuleFor(x => x.TargetUrl)
                .Must(BeValidUrl)
                .WithMessage("Target URL must be a valid URL")
                .When(x => !string.IsNullOrWhiteSpace(x.TargetUrl));

            RuleFor(x => x.UrlTitle)
                .MaximumLength(255)
                .WithMessage("Url title is invalid")
                .When(x => !string.IsNullOrWhiteSpace(x.UrlTitle));

            RuleFor(x => x.LogoId)
                .GreaterThan(0);

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