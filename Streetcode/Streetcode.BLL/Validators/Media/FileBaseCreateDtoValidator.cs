using FluentValidation;
using Streetcode.BLL.DTO.Media;

namespace Streetcode.BLL.Validators.Media
{
    /// <summary>
    /// Validator for FileBaseCreateDTO.
    /// </summary>
    public class FileBaseCreateDtoValidator : AbstractValidator<FileBaseCreateDTO>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FileBaseCreateDtoValidator"/> class.
        /// </summary>
        public FileBaseCreateDtoValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty()
                .MaximumLength(255);

            RuleFor(x => x.BaseFormat)
                .NotEmpty();

            RuleFor(x => x.MimeType)
                .NotEmpty()
                .MaximumLength(100);

            RuleFor(x => x.Extension)
                .NotEmpty()
                .MaximumLength(10);
        }
    }
}
