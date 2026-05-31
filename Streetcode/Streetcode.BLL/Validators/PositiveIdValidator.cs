using FluentValidation;
using Streetcode.BLL.MediatR.Interface;
using Streetcode.BLL.Resources;

namespace Streetcode.BLL.Validators
{
    public class PositiveIdValidator<T> : AbstractValidator<T>
        where T : IHasId
    {
        public PositiveIdValidator()
        {
            RuleLevelCascadeMode = CascadeMode.Stop;

            RuleFor(x => x.Id)
                .GreaterThan(0)
                .WithMessage(ErrorMessages.IdMustBePositive);
        }
    }
}