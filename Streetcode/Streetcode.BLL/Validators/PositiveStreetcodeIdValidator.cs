using FluentValidation;
using Streetcode.BLL.MediatR.Interface;

namespace Streetcode.BLL.Validators
{
    public class PositiveStreetcodeIdValidator<T> : AbstractValidator<T>
        where T : IHasStreetcodeId
    {
        public PositiveStreetcodeIdValidator()
        {
            RuleFor(x => x.StreetcodeId).GreaterThan(0).WithMessage("StreetcodeId must be positive.");
        }
    }
}
