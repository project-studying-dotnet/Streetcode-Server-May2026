using FluentValidation;
using Streetcode.BLL.MediatR.Streetcode.RelatedTerm.Delete;
using Streetcode.BLL.Resources;

namespace Streetcode.BLL.Validators.Streetcode.RelatedTerm.Delete
{
    public class DeleteRelatedTermCommandValidator : AbstractValidator<DeleteRelatedTermCommand>
    {
        private const int MaxWordLength = 255;

        public DeleteRelatedTermCommandValidator()
        {
            RuleLevelCascadeMode = CascadeMode.Stop;

            RuleFor(x => x.Word)
                .NotEmpty()
                .WithMessage(ErrorMessages.WordIsRequired)
                .MaximumLength(MaxWordLength)
                .WithMessage(string.Format(ErrorMessages.WordMustNotExceedCharacters, MaxWordLength));

            RuleFor(x => x.TermId)
                .GreaterThan(0)
                .WithMessage(ErrorMessages.TermIdMustBePositive);
        }
    }
}