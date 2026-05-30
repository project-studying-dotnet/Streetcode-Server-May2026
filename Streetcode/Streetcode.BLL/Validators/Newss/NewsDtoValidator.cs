using FluentValidation;
using Streetcode.BLL.DTO.News;
using Streetcode.BLL.Resources;

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
            RuleLevelCascadeMode = CascadeMode.Stop;

            RuleFor(x => x.Title)
                .NotEmpty().WithMessage(ErrorMessages.TitleIsRequired)
                .MaximumLength(TitleMaxLength).WithMessage(string.Format(ErrorMessages.TitleMustNotExceedCharacters, TitleMaxLength));

            RuleFor(x => x.Text)
                .NotEmpty().WithMessage(ErrorMessages.TextIsRequired)
                .MaximumLength(TextMaxLength).WithMessage(string.Format(ErrorMessages.TextMustNotExceedCharacters, TextMaxLength));

            RuleFor(x => x.URL)
                .NotEmpty().WithMessage(ErrorMessages.UrlIsRequired);

            RuleFor(x => x.ImageId)
                .NotEmpty().WithMessage(ErrorMessages.ImageIsRequired);
        }
    }
}
