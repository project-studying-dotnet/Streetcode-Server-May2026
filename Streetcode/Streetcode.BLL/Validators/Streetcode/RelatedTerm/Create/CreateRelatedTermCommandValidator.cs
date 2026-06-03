using FluentValidation;
using Streetcode.BLL.MediatR.Streetcode.RelatedTerm.Create;
using Streetcode.BLL.Resources;

namespace Streetcode.BLL.Validators.Streetcode.RelatedTerm.Create
{
    public class CreateRelatedTermCommandValidator : AbstractValidator<CreateRelatedTermCommand>
    {
        public CreateRelatedTermCommandValidator()
        {
            RuleLevelCascadeMode = CascadeMode.Stop;

            RuleFor(x => x.CreateRelatedTerm)
                .NotNull()
                .WithMessage(ErrorMessages.RelatedTermIsRequired)
                .SetValidator(new CreateRelatedTermDtoValidator());
        }
    }
}