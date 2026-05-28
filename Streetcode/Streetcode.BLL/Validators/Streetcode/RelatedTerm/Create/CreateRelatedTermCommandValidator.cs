using FluentValidation;
using Streetcode.BLL.MediatR.Streetcode.RelatedTerm.Create;
using Streetcode.BLL.Resources;

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
            RuleLevelCascadeMode = CascadeMode.Stop;

            RuleFor(x => x.RelatedTerm)
                .NotNull()
                .WithMessage(ErrorMessages.RelatedTermIsRequired)
                .SetValidator(new RelatedTermDtoValidator());
        }
    }
}