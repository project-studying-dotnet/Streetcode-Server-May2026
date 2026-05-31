using FluentValidation;
using Streetcode.BLL.DTO.Partners.Create;
using Streetcode.BLL.Resources;

namespace Streetcode.BLL.Validators.Partners.Create
{
    public class CreatePartnerSourceLinkDtoValidator : AbstractValidator<CreatePartnerSourceLinkDTO>
    {
        private const int MaxUrlLength = 500;

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