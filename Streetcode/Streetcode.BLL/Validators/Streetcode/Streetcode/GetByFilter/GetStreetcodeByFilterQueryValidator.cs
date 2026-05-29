using FluentValidation;
using Streetcode.BLL.MediatR.Streetcode.Streetcode.GetByFilter;
using Streetcode.BLL.Resources;

namespace Streetcode.BLL.Validators.Streetcode.Streetcode.GetByFilter
{
    public class GetStreetcodeByFilterQueryValidator
        : AbstractValidator<GetStreetcodeByFilterQuery>
    {
        public GetStreetcodeByFilterQueryValidator()
        {
            RuleLevelCascadeMode = CascadeMode.Stop;

            RuleFor(x => x.Filter)
                .NotNull()
                .WithMessage(ErrorMessages.FilterIsRequired)
                .SetValidator(new StreetcodeFilterRequestDtoValidator());
        }
    }
}