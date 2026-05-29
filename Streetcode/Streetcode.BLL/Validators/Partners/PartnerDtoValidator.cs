using FluentValidation;
using Streetcode.BLL.DTO.Partners;
using Streetcode.BLL.Resources;

namespace Streetcode.BLL.Validators.Partners
{
    public class PartnerDtoValidator : AbstractValidator<PartnerDTO>
    {
        private const int MaxDescriptionLength = 400;

        public PartnerDtoValidator()
        {
            RuleLevelCascadeMode = CascadeMode.Stop;

            RuleFor(x => x.Id)
                .GreaterThanOrEqualTo(0)
                .WithMessage(ErrorMessages.IdMustBeGreaterOrEqualToZero);

            RuleFor(x => x.Title)
                .NotEmpty()
                .WithMessage(ErrorMessages.TitleIsRequired);

            RuleFor(x => x.Description)
                .MaximumLength(MaxDescriptionLength)
                .WithMessage(string.Format(ErrorMessages.DescriptionMustNotExceedCharacters, MaxDescriptionLength))
                .When(x => !string.IsNullOrWhiteSpace(x.Description));

            RuleFor(x => x.LogoId)
                .GreaterThan(0)
                .WithMessage(ErrorMessages.LogoIdMustBePositive);

            RuleFor(x => x.TargetUrl)
                .NotNull()
                .WithMessage(ErrorMessages.TargetUrlIsRequired);

            RuleFor(x => x.PartnerSourceLinks)
                .NotNull()
                .WithMessage(ErrorMessages.PartnerSourceLinksRequired);

            RuleFor(x => x.Streetcodes)
                .NotNull()
                .WithMessage(ErrorMessages.StreetcodesRequired);
        }
    }
}