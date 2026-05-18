using FluentValidation;
using Streetcode.BLL.DTO.Partners;

namespace Streetcode.BLL.Validators.Partners
{
    /// <summary>
    /// Validator for <see cref="PartnerDTO"/>.
    /// </summary>
    public class PartnerDtoValidator : AbstractValidator<PartnerDTO>
    {
        private const int DescriptionMaxLength = 400;

        /// <summary>
        /// Initializes a new instance of the <see cref="PartnerDtoValidator"/> class.
        /// </summary>
        public PartnerDtoValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThanOrEqualTo(0)
                .WithMessage("Id must be greater than or equal to 0");

            RuleFor(x => x.Title)
                .NotEmpty()
                .WithMessage("Title is required");

            RuleFor(x => x.Description)
                .MaximumLength(DescriptionMaxLength)
                .WithMessage($"Description must not exceed {DescriptionMaxLength} characters")
                .When(x => !string.IsNullOrWhiteSpace(x.Description));

            RuleFor(x => x.LogoId)
                .GreaterThan(0)
                .WithMessage("LogoId must be greater than 0");

            RuleFor(x => x.TargetUrl)
                .NotNull()
                .WithMessage("Target URL is required");

            RuleFor(x => x.PartnerSourceLinks)
                .NotNull()
                .WithMessage("Partner source links collection is required");

            RuleFor(x => x.Streetcodes)
                .NotNull()
                .WithMessage("Streetcodes collection is required");
        }
    }
}