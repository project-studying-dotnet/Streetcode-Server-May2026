using FluentValidation;
using Streetcode.BLL.MediatR.Newss.Update;
using Streetcode.BLL.Resources;

namespace Streetcode.BLL.Validators.Newss.Update
{
    public class UpdateNewsCommandValidator : AbstractValidator<UpdateNewsCommand>
    {
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