using FluentValidation;
using Streetcode.BLL.DTO.Streetcode.TextContent;

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
            RuleFor(x => x.Word)
                .NotEmpty();

            RuleFor(x => x.TermId)
                .GreaterThan(0);

            RuleFor(x => x.Id)
                .GreaterThanOrEqualTo(0);
        }
    }
}