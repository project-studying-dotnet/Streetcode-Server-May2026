using FluentValidation;
using Streetcode.BLL.MediatR.Media.Image.Create;
using Streetcode.BLL.Resources;

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
            RuleLevelCascadeMode = CascadeMode.Stop;

            RuleFor(x => x.Image)
                .NotNull()
                .WithMessage(ErrorMessages.ImageIsRequired)
                .SetValidator(new ImageFileBaseCreateDtoValidator());
        }
    }
}
