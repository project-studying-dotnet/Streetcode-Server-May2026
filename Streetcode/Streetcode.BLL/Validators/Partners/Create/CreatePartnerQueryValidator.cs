using FluentValidation;
using Streetcode.BLL.MediatR.Partners.Create;
using Streetcode.BLL.Resources;

namespace Streetcode.BLL.Validators.Partners.Create
{
    public class CreatePartnerQueryValidator : AbstractValidator<CreatePartnerQuery>
    {
        public CreatePartnerQueryValidator()
        {
            RuleLevelCascadeMode = CascadeMode.Stop;

            RuleFor(x => x.newPartner)
                .NotNull()
                .WithMessage(ErrorMessages.PartnerIsRequired)
                .SetValidator(new CreatePartnerDtoValidator());
        }
    }
}