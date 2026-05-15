using FluentValidation;
using Streetcode.BLL.DTO.Media;

namespace Streetcode.BLL.Validators.Media.Audio
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
                .MaximumLength(200)
                .WithMessage("Title must not exceed 200 characters");

            RuleFor(x => x.BaseFormat)
                .MaximumLength(50)
                .WithMessage("BaseFormat must not exceed 50 characters");

            RuleFor(x => x.MimeType)
                .MaximumLength(100)
                .WithMessage("MimeType must not exceed 100 characters");

            RuleFor(x => x.Extension)
                .MaximumLength(10)
                .WithMessage("Extension must not exceed 10 characters");
        }
    }
}
