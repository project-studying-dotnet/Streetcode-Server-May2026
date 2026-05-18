using FluentValidation;
using Streetcode.BLL.MediatR.Newss.Create;

namespace Streetcode.BLL.Validators.Newss.Create
{
    /// <summary>
    /// Validator for CreateNewsCommand.
    /// </summary>
    public class CreateNewsCommandValidator : AbstractValidator<CreateNewsCommand>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CreateNewsCommandValidator"/> class.
        /// </summary>
        public CreateNewsCommandValidator()
        {
            RuleFor(x => x.newNews)
                .NotNull()
                .WithMessage("News is required")
                .SetValidator(new NewsDtoValidator());
        }
    }
}
