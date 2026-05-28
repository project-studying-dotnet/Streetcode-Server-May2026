using FluentValidation;
using Streetcode.BLL.DTO.Media;
using Streetcode.BLL.Resources;

namespace Streetcode.BLL.Validators.Media
{
    /// <summary>
    /// Validator for FileBaseCreateDTO.
    /// </summary>
    public class FileBaseCreateDtoValidator : AbstractValidator<FileBaseCreateDTO>
    {
        private const int MaxTitleLength = 255;
        private const int MaxMimeTypeLength = 100;
        private const int MaxExtensionLength = 10;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileBaseCreateDtoValidator"/> class.
        /// </summary>
        public FileBaseCreateDtoValidator()
        {
            RuleLevelCascadeMode = CascadeMode.Stop;

            RuleFor(x => x.Title)
                .NotEmpty().WithMessage(ErrorMessages.TitleIsRequired)
                .MaximumLength(MaxTitleLength).WithMessage(string.Format(ErrorMessages.TitleMustNotExceedCharacters, MaxTitleLength));

            RuleFor(x => x.BaseFormat)
                .NotEmpty().WithMessage(ErrorMessages.BaseFormatIsRequired);

            RuleFor(x => x.MimeType)
                .NotEmpty().WithMessage(ErrorMessages.MimeTypeIsRequired)
                .MaximumLength(MaxMimeTypeLength).WithMessage(string.Format(ErrorMessages.MimeTypeMustNotExceedCharacters, MaxMimeTypeLength));

            RuleFor(x => x.Extension)
                .NotEmpty().WithMessage(ErrorMessages.ExtensionIsRequired)
                .MaximumLength(MaxExtensionLength).WithMessage(string.Format(ErrorMessages.ExtensionMustNotExceedCharacters, MaxExtensionLength));
        }
    }
}
