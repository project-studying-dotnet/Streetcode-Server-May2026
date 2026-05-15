using FluentValidation;
using Streetcode.BLL.DTO.Media.Images;

namespace Streetcode.BLL.Validators.Media.Image.Create
{
    /// <summary>
    /// Validator for ImageFileBaseCreateDTO.
    /// </summary>
    public class ImageFileBaseCreateDtoValidator : AbstractValidator<ImageFileBaseCreateDTO>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ImageFileBaseCreateDtoValidator"/> class.
        /// </summary>
        public ImageFileBaseCreateDtoValidator()
        {
            Include(new FileBaseCreateDtoValidator());

            RuleFor(x => x.Alt)
                .MaximumLength(255);
        }
    }
}
