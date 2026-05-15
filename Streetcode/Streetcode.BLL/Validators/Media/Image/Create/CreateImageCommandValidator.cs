using FluentValidation;
using Streetcode.BLL.MediatR.Media.Image.Create;

namespace Streetcode.BLL.Validators.Media.Image.Create
{
    /// <summary>
    /// Validator for CreateImageCommand.
    /// </summary>
    public class CreateImageCommandValidator : AbstractValidator<CreateImageCommand>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CreateImageCommandValidator"/> class.
        /// </summary>
        public CreateImageCommandValidator()
        {
            RuleFor(x => x.Image)
                .NotNull()
                .WithMessage("Image is required")
                .SetValidator(new ImageFileBaseCreateDtoValidator());
        }
    }
}
