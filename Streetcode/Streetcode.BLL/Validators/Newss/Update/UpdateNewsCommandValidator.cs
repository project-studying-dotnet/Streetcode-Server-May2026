using FluentValidation;
using Streetcode.BLL.MediatR.Newss.Update;

namespace Streetcode.BLL.Validators.Newss.Update
{
    /// <summary>
    /// Validator for UpdateNewsCommand.
    /// </summary>
    public class UpdateNewsCommandValidator : AbstractValidator<UpdateNewsCommand>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateNewsCommandValidator"/> class.
        /// </summary>
        public UpdateNewsCommandValidator()
        {
            RuleFor(x => x.news)
                .NotNull()
                .WithMessage("News is required")
                .SetValidator(new NewsDtoValidator());

            RuleFor(x => x.news.Id)
                .GreaterThan(0);
        }
    }
}