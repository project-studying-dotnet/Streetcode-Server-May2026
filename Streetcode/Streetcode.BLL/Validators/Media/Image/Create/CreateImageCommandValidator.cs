using FluentValidation;
using Streetcode.BLL.MediatR.Media.Image.Create;
using Streetcode.BLL.Resources;

namespace Streetcode.BLL.Validators.Media.Image.Create
{
    public class CreateImageCommandValidator : AbstractValidator<CreateImageCommand>
    {
        public CreateImageCommandValidator()
        {
            RuleLevelCascadeMode = CascadeMode.Stop;

            RuleFor(x => x.Image)
                .NotNull()
                .WithMessage(ErrorMessages.ImageIsRequired)
                .SetValidator(new ImageFileBaseCreateDtoValidator());
        }
    }
}
