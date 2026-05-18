using FluentValidation;
using Streetcode.BLL.DTO.Partners.Create;

namespace Streetcode.BLL.Validators.Partners.Create
{
    /// <summary>
    /// Validator for CreatePartnerSourceLinkDTO.
    /// </summary>
    public class CreatePartnerSourceLinkDtoValidator : AbstractValidator<CreatePartnerSourceLinkDTO>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CreatePartnerSourceLinkDtoValidator"/> class.
        /// </summary>
        public CreatePartnerSourceLinkDtoValidator()
        {
            RuleFor(x => x.LogoType)
                .IsInEnum();

            RuleFor(x => x.TargetUrl)
                .NotEmpty()
                .MaximumLength(500)
                .Must(uri => Uri.IsWellFormedUriString(uri, UriKind.Absolute))
                .WithMessage("TargetUrl must be a valid absolute URL");
        }
    }
}