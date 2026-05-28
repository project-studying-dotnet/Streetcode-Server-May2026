using FluentValidation;
using Streetcode.BLL.MediatR.Streetcode.RelatedTerm.Update;
using Streetcode.BLL.Resources;

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
            RuleLevelCascadeMode = CascadeMode.Stop;

            RuleFor(x => x.id)
                .GreaterThan(0)
                .WithMessage(ErrorMessages.IdMustBePositive);

            RuleFor(x => x.RelatedTerm)
                .NotNull()
                .WithMessage(ErrorMessages.RelatedTermIsRequired)
                .SetValidator(new RelatedTermDtoValidator());

            RuleFor(x => x.RelatedTerm.Id)
                .GreaterThan(0)
                .WithMessage(ErrorMessages.IdMustBePositive);
        }
    }
}