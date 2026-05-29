using FluentValidation;
using Streetcode.BLL.DTO.Media.Images;
using Streetcode.BLL.Resources;

namespace Streetcode.BLL.Validators.Media.Image.Create
{
    public class ImageFileBaseCreateDtoValidator : AbstractValidator<ImageFileBaseCreateDTO>
    {
        private const int MaxAltLength = 255;
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
