using FluentValidation;
using Streetcode.BLL.MediatR.Partners.Create;
using Streetcode.BLL.Resources;

namespace Streetcode.BLL.Validators.Partners.Create
{
    /// <summary>
    /// Validator for CreatePartnerQuery.
    /// </summary>
    public class CreatePartnerQueryValidator : AbstractValidator<CreatePartnerQuery>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CreatePartnerQueryValidator"/> class.
        /// </summary>
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