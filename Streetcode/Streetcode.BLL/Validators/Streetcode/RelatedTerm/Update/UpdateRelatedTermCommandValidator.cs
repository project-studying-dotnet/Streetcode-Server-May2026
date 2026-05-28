using FluentValidation;
using Streetcode.BLL.MediatR.Streetcode.RelatedTerm.Update;

namespace Streetcode.BLL.Validators.Streetcode.RelatedTerm.Update
{
    /// <summary>
    /// Validator for UpdateRelatedTermCommand.
    /// </summary>
    public class UpdateRelatedTermCommandValidator : AbstractValidator<UpdateRelatedTermCommand>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateRelatedTermCommandValidator"/> class.
        /// </summary>
        public UpdateRelatedTermCommandValidator()
        {
            RuleFor(x => x.id)
                .GreaterThan(0);

            RuleFor(x => x.RelatedTerm)
                .NotNull()
                .WithMessage("RelatedTerm is required")
                .SetValidator(new RelatedTermDtoValidator());

            RuleFor(x => x.RelatedTerm.Id)
                .GreaterThan(0)
                .When(x => x.RelatedTerm != null);
        }
    }
}