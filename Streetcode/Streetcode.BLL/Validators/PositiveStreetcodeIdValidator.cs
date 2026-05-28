using FluentValidation;
using Streetcode.BLL.MediatR.Interface;
using Streetcode.BLL.Resources;

namespace Streetcode.BLL.Validators
{
    public class PositiveStreetcodeIdValidator<T> : AbstractValidator<T>
        where T : IHasStreetcodeId
    {
        public PositiveStreetcodeIdValidator()
        {
            RuleLevelCascadeMode = CascadeMode.Stop;

            RuleFor(x => x.StreetcodeId)
                .GreaterThan(0)
                .WithMessage(ErrorMessages.StreetcodeIdMustBePositive);
        }
    }
}