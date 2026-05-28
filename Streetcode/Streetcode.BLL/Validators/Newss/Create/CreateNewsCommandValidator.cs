using FluentValidation;
using Streetcode.BLL.MediatR.Newss.Create;
using Streetcode.BLL.Resources;

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
            RuleLevelCascadeMode = CascadeMode.Stop;

            RuleFor(x => x.newNews)
                .NotNull()
                .WithMessage(ErrorMessages.NewsIsRequired)
                .SetValidator(new NewsDtoValidator());
        }
    }
}