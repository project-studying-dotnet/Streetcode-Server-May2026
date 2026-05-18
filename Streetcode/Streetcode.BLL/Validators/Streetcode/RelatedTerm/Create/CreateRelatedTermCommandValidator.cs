using FluentValidation;
using Streetcode.BLL.MediatR.Streetcode.RelatedTerm.Create;

namespace Streetcode.BLL.Validators.Streetcode.RelatedTerm.Create
{
    /// <summary>
    /// Validator for CreateRelatedTermCommand.
    /// </summary>
    public class CreateRelatedTermCommandValidator : AbstractValidator<CreateRelatedTermCommand>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CreateRelatedTermCommandValidator"/> class.
        /// </summary>
        public CreateRelatedTermCommandValidator()
        {
            RuleFor(x => x.RelatedTerm)
                .NotNull()
                .WithMessage("RelatedTerm is required")
                .SetValidator(new RelatedTermDtoValidator());
        }
    }
}