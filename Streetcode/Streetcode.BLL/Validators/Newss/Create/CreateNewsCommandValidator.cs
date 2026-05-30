using FluentValidation;
using Streetcode.BLL.MediatR.Newss.Create;
using Streetcode.BLL.Resources;

namespace Streetcode.BLL.Validators.Newss.Create
{
    public class CreateNewsCommandValidator : AbstractValidator<CreateNewsCommand>
    {
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