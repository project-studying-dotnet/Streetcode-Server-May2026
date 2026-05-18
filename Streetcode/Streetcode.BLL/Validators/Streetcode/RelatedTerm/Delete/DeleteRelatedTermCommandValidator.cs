using FluentValidation;
using Streetcode.BLL.MediatR.Streetcode.RelatedTerm.Delete;

namespace Streetcode.BLL.Validators.Streetcode.RelatedTerm.Delete
{
    /// <summary>
    /// Validator for DeleteRelatedTermCommand.
    /// </summary>
    public class DeleteRelatedTermCommandValidator : AbstractValidator<DeleteRelatedTermCommand>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DeleteRelatedTermCommandValidator"/> class.
        /// </summary>
        public DeleteRelatedTermCommandValidator()
        {
            RuleFor(x => x.word)
                .NotEmpty()
                .MaximumLength(255);
        }
    }
}
