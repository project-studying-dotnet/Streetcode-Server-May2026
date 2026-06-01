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

            RuleFor(x => x.word)
                .NotEmpty()
                .WithMessage(ErrorMessages.WordIsRequired)
                .MaximumLength(MaxWordLength)
                .WithMessage(string.Format(ErrorMessages.WordMustNotExceedCharacters, MaxWordLength));
        }
    }
}