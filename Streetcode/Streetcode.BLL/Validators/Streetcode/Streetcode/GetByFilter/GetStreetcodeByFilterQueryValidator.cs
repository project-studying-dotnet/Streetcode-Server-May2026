using FluentValidation;
using Streetcode.BLL.MediatR.Streetcode.Streetcode.GetByFilter;
using Streetcode.BLL.Resources;

namespace Streetcode.BLL.Validators.Streetcode.Streetcode.GetByFilter
{
    /// <summary>
    /// Validator for GetStreetcodeByFilterQuery.
    /// </summary>
    public class GetStreetcodeByFilterQueryValidator
        : AbstractValidator<GetStreetcodeByFilterQuery>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GetStreetcodeByFilterQueryValidator"/> class.
        /// </summary>
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