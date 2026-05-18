using FluentValidation;
using Streetcode.BLL.MediatR.Streetcode.Streetcode.GetByIndex;

namespace Streetcode.BLL.Validators.Streetcode.Streetcode.GetByIndex
{
    /// <summary>
    /// Validator for <see cref="GetStreetcodeByIndexQuery"/>.
    /// </summary>
    public class GetStreetcodeByIndexQueryValidator : AbstractValidator<GetStreetcodeByIndexQuery>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GetStreetcodeByIndexQueryValidator"/> class.
        /// </summary>
        public GetStreetcodeByIndexQueryValidator()
        {
            RuleFor(x => x.Index)
                .GreaterThanOrEqualTo(0)
                .WithMessage("Index must be greater than or equal to 0");
        }
    }
}
