using FluentValidation;
using Streetcode.BLL.MediatR.Streetcode.Streetcode.GetByIndex;
using Streetcode.BLL.Resources;

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
            RuleLevelCascadeMode = CascadeMode.Stop;

            RuleFor(x => x.Index)
                .GreaterThanOrEqualTo(0)
                .WithMessage(ErrorMessages.IndexMustBeGreaterOrEqualToZero);
        }
    }
}