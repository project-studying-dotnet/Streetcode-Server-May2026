using FluentValidation;
using Streetcode.BLL.MediatR.Interface;

namespace Streetcode.BLL.Validators
{
    public class PositiveIdValidator<T> : AbstractValidator<T>
        where T : IHasId
    {
        public PositiveIdValidator()
        {
            RuleFor(x => x.Id).GreaterThan(0).WithMessage("The identifier must be positive.");
        }
    }
}
