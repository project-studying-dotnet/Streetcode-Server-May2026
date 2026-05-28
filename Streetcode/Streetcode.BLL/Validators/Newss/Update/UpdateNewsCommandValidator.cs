using FluentValidation;
using Streetcode.BLL.MediatR.Newss.Update;
using Streetcode.BLL.Resources;

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
            RuleLevelCascadeMode = CascadeMode.Stop;

            RuleFor(x => x.news)
                .NotNull()
                .WithMessage(ErrorMessages.NewsIsRequired)
                .SetValidator(new NewsDtoValidator());

            RuleFor(x => x.news.Id)
                .GreaterThan(0)
                .WithMessage(ErrorMessages.IdMustBePositive);
        }
    }
}