using FluentValidation;
using Streetcode.BLL.DTO.Partners.Create;
using Streetcode.BLL.Resources;

namespace Streetcode.BLL.Validators.Partners.Create
{
    /// <summary>
    /// Validator for CreatePartnerSourceLinkDTO.
    /// </summary>
    public class CreatePartnerSourceLinkDtoValidator : AbstractValidator<CreatePartnerSourceLinkDTO>
    {
        private const int MaxUrlLength = 500;

        /// <summary>
        /// Initializes a new instance of the <see cref="CreatePartnerSourceLinkDtoValidator"/> class.
        /// </summary>
        public CreatePartnerSourceLinkDtoValidator()
        {
            RuleLevelCascadeMode = CascadeMode.Stop;

            RuleFor(x => x.LogoType)
                .IsInEnum()
                .WithMessage(ErrorMessages.LogoTypeMustBeValidEnum);

            RuleFor(x => x.TargetUrl)
                .NotEmpty()
                .WithMessage(ErrorMessages.TargetUrlIsRequired)
                .MaximumLength(MaxUrlLength)
                .WithMessage(string.Format(ErrorMessages.TargetUrlMustNotExceedCharacters, MaxUrlLength))
                .Must(uri => Uri.IsWellFormedUriString(uri, UriKind.Absolute))
                .WithMessage(ErrorMessages.TargetUrlMustBeValidAbsoluteUrl);
        }
    }
}