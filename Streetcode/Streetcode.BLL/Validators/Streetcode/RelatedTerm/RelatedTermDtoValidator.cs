using FluentValidation;
using Streetcode.BLL.DTO.Streetcode.TextContent;
using Streetcode.BLL.Resources;

namespace Streetcode.BLL.Validators.Streetcode.RelatedTerm
{
    /// <summary>
    /// Validator for RelatedTermDTO.
    /// </summary>
    public class RelatedTermDtoValidator : AbstractValidator<RelatedTermDTO>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RelatedTermDtoValidator"/> class.
        /// </summary>
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