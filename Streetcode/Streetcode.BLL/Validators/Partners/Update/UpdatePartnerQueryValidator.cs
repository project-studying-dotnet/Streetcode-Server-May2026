using FluentValidation;
using Streetcode.BLL.MediatR.Partners.Update;
using Streetcode.BLL.Resources;
using Streetcode.BLL.Validators.Partners.Create;

namespace Streetcode.BLL.Validators.Partners.Update
{
    public class UpdatePartnerQueryValidator : AbstractValidator<UpdatePartnerQuery>
    {
        public UpdatePartnerQueryValidator()
        {
            RuleLevelCascadeMode = CascadeMode.Stop;

            RuleFor(x => x.Partner)
                .NotNull()
                .WithMessage(ErrorMessages.PartnerIsRequired)
                .SetValidator(new CreatePartnerDtoValidator());

            RuleFor(x => x.Partner.Id)
                .GreaterThan(0)
                .WithMessage(ErrorMessages.IdMustBePositive);
        }
    }
}