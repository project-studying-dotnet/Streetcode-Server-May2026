using FluentValidation;
using Streetcode.BLL.MediatR.Partners.Create;

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
            RuleFor(x => x.newPartner)
                .NotNull()
                .WithMessage("Partner is required")
                .SetValidator(new CreatePartnerDtoValidator());
        }
    }
}