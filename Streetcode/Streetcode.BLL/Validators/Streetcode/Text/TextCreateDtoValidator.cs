using FluentValidation;
using Streetcode.BLL.DTO.Streetcode.TextContent.Text;
using Streetcode.BLL.Resources;

namespace Streetcode.XUnitTest.Validators.Streetcode.Text
{
    public class TextCreateDtoValidator : AbstractValidator<TextCreateDto>
    {
        private const int AdditionalTextMaxLength = 250;
        public TextCreateDtoValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty()
                .WithMessage(ErrorMessages.TitleIsRequired);
            RuleFor(x => x.TextContent)
                .NotEmpty()
                .WithMessage(ErrorMessages.TextContentIsRequired);
            RuleFor(x => x.AdditionalText)
                .MaximumLength(AdditionalTextMaxLength)
                .WithMessage(string.Format(ErrorMessages.AdditionalTextMustNotExceed, AdditionalTextMaxLength))
                .When(x => !string.IsNullOrWhiteSpace(x.AdditionalText));
        }
    }
}