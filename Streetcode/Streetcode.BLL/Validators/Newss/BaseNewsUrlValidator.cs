using FluentValidation;
using Streetcode.BLL.Resources;

namespace Streetcode.BLL.Validators.Newss;
public abstract class BaseNewsUrlValidator<T> : AbstractValidator<T> where T : IUrlQuery
{
    protected BaseNewsUrlValidator()
    {
        RuleFor(x => x.Url)
            .NotEmpty().WithMessage(ErrorMessages.UrlIsRequired)
            .MaximumLength(2048).WithMessage(string.Format(ErrorMessages.UrlMustNotExceedCharacters, 2048));
    }
}