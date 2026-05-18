using FluentValidation;
using Streetcode.BLL.MediatR.Streetcode.Streetcode.GetByFilter;

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
            RuleFor(x => x.Filter)
                .NotNull()
                .WithMessage("Filter is required")
                .SetValidator(new StreetcodeFilterRequestDtoValidator());
        }
    }
}