using FluentValidation;
using Streetcode.BLL.MediatR.Streetcode.RelatedTerm.Update;
using Streetcode.BLL.Resources;

namespace Streetcode.BLL.Validators.Streetcode.RelatedTerm.Update
{
    public class UpdateRelatedTermCommandValidator : AbstractValidator<UpdateRelatedTermCommand>
    {
        public UpdateRelatedTermCommandValidator()
        {
            RuleLevelCascadeMode = CascadeMode.Stop;

            RuleFor(x => x.id)
                .GreaterThan(0)
                .WithMessage(ErrorMessages.IdMustBePositive);

            RuleFor(x => x.RelatedTerm)
                .NotNull().WithMessage(ErrorMessages.RelatedTermIsRequired)
                .SetValidator(new RelatedTermDtoValidator())
                .ChildRules(term =>
                {
                    term.RuleFor(x => x.Id)
                        .GreaterThan(0)
                        .WithMessage(ErrorMessages.IdMustBePositive);
                });
        }
    }
}