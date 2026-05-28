using FluentValidation;
using Streetcode.BLL.DTO.Media.Images;
using Streetcode.BLL.Resources;

namespace Streetcode.BLL.Validators.Media.Image.Create
{
    /// <summary>
    /// Validator for ImageFileBaseCreateDTO.
    /// </summary>
    public class ImageFileBaseCreateDtoValidator : AbstractValidator<ImageFileBaseCreateDTO>
    {
        private const int MaxAltLength = 255;

        /// <summary>
        /// Initializes a new instance of the <see cref="ImageFileBaseCreateDtoValidator"/> class.
        /// </summary>
        public ImageFileBaseCreateDtoValidator()
        {
            RuleLevelCascadeMode = CascadeMode.Stop;

            Include(new FileBaseCreateDtoValidator());

            RuleFor(x => x.Alt)
                .MaximumLength(MaxAltLength)
                .WithMessage(string.Format(ErrorMessages.AltMustNotExceedCharacters, MaxAltLength));
        }
    }
}
