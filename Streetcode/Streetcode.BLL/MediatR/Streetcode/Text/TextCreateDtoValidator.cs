using FluentValidation;
using Streetcode.BLL.DTO.Streetcode.TextContent.Text;

namespace Streetcode.BLL.MediatR.Streetcode.Text
{
    /// <summary>
    /// Validator for <see cref="TextCreateDTO"/>.
    /// </summary>
    public class TextCreateDtoValidator : AbstractValidator<TextCreateDto>
    {
        private const int AdditionalTextMaxLength = 250;

        /// <summary>
        /// Initializes a new instance of the <see cref="TextCreateDtoValidator"/> class.
        /// </summary>
        public TextCreateDtoValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty()
                .WithMessage("Title is required");

            RuleFor(x => x.TextContent)
                .NotEmpty()
                .WithMessage("Text content is required");

            RuleFor(x => x.AdditionalText)
                .MaximumLength(AdditionalTextMaxLength)
                .WithMessage($"Additional text must not exceed {AdditionalTextMaxLength} characters")
                .When(x => !string.IsNullOrWhiteSpace(x.AdditionalText));
        }
    }
}