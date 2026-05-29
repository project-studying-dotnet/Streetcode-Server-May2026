using FluentValidation;
using Streetcode.BLL.MediatR.Streetcode.Streetcode.GetByIndex;
using Streetcode.BLL.Resources;

namespace Streetcode.BLL.Validators.Streetcode.Streetcode.GetByIndex
{
    public class GetStreetcodeByIndexQueryValidator : AbstractValidator<GetStreetcodeByIndexQuery>
    {
        public GetStreetcodeByIndexQueryValidator()
        {
            RuleLevelCascadeMode = CascadeMode.Stop;

            RuleFor(x => x.Index)
                .GreaterThanOrEqualTo(0)
                .WithMessage(ErrorMessages.IndexMustBeGreaterOrEqualToZero);
        }
    }
}