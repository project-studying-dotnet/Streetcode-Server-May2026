using FluentValidation;
using Streetcode.BLL.DTO.Streetcode.TextContent.Term;

namespace Streetcode.BLL.Validators.Streetcode.Term
{
    /// <summary>
    /// Validator for TermDTO.
    /// </summary>
    public class TermDtoValidator : AbstractValidator<TermDto>
    {
        private const int DescriptionMaxLength = 250;

        /// <summary>
        /// Initializes a new instance of the <see cref="TermDtoValidator"/> class.
        /// </summary>
        public TermDtoValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty()
                .WithMessage("Title is required");

            RuleFor(x => x.Description)
                .NotEmpty()
                .WithMessage("Description is required")
                .MaximumLength(DescriptionMaxLength)
                .WithMessage($"Description must not exceed {DescriptionMaxLength} characters");
        }
    }
}
