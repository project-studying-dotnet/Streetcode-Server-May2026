using FluentValidation;
using Streetcode.BLL.DTO.Streetcode.TextContent.Term;
using Streetcode.BLL.Resources;

namespace Streetcode.BLL.Validators.Streetcode.Term
{
    /// <summary>
    /// Validator for TermDTO.
    /// </summary>
    public class TermDtoValidator : AbstractValidator<TermDto>
    {
        private const int MaxDescriptionLength = 250;

        /// <summary>
        /// Initializes a new instance of the <see cref="TermDtoValidator"/> class.
        /// </summary>
        public TermDtoValidator()
        {
            RuleLevelCascadeMode = CascadeMode.Stop;

            RuleFor(x => x.Title)
                .NotEmpty()
                .WithMessage(ErrorMessages.TitleIsRequired);

            RuleFor(x => x.Description)
                .NotEmpty()
                .WithMessage(ErrorMessages.DescriptionIsRequired)
                .MaximumLength(MaxDescriptionLength)
                .WithMessage(string.Format(ErrorMessages.DescriptionMustNotExceedCharacters, MaxDescriptionLength));
        }
    }
}