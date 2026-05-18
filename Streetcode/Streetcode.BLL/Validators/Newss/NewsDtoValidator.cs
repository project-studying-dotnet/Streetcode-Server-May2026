using FluentValidation;
using Streetcode.BLL.DTO.News;

namespace Streetcode.BLL.Validators.Newss
{
    /// <summary>
    /// Validator for NewsDTO.
    /// </summary>
    public class NewsDtoValidator : AbstractValidator<NewsDTO>
    {
        private const int TitleMaxLength = 40;
        private const int TextMaxLength = 450;

        /// <summary>
        /// Initializes a new instance of the <see cref="NewsDtoValidator"/> class.
        /// </summary>
        public NewsDtoValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty()
                .WithMessage("Title is required")
                .MaximumLength(TitleMaxLength)
                .WithMessage($"Title must not exceed {TitleMaxLength} characters");

            RuleFor(x => x.Text)
                .NotEmpty()
                .WithMessage("Text is required")
                .MaximumLength(TextMaxLength)
                .WithMessage($"Text must not exceed {TextMaxLength} characters");

            RuleFor(x => x.URL)
                .NotEmpty()
                .WithMessage("URL is required");

            RuleFor(x => x.ImageId)
                .NotNull()
                .WithMessage("Image is required");
        }
    }
}
