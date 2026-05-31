using FluentValidation;
using Streetcode.BLL.DTO.Streetcode.TextContent;
using Streetcode.BLL.Resources;

namespace Streetcode.BLL.Validators.Streetcode.RelatedTerm
{
    public class RelatedTermDtoValidator : AbstractValidator<RelatedTermDTO>
    {
        public RelatedTermDtoValidator()
        {
            RuleLevelCascadeMode = CascadeMode.Stop;

            RuleFor(x => x.Word)
                .NotEmpty()
                .WithMessage(ErrorMessages.WordIsRequired);

            RuleFor(x => x.TermId)
                .GreaterThan(0)
                .WithMessage(ErrorMessages.TermIdMustBePositive);

            RuleFor(x => x.Id)
                .GreaterThanOrEqualTo(0)
                .WithMessage(ErrorMessages.IdMustBeGreaterOrEqualToZero);
        }
    }
}