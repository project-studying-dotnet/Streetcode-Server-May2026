using FluentValidation;
using Streetcode.BLL.MediatR.Streetcode.Streetcode.GetAllCatalog;
using Streetcode.BLL.Resources;

namespace Streetcode.BLL.Validators.Streetcode.Streetcode.GetAllCatalog
{
    /// <summary>
    /// Validator for GetAllStreetcodesCatalogQuery.
    /// </summary>
    public class GetAllStreetcodesCatalogQueryValidator
        : AbstractValidator<GetAllStreetcodesCatalogQuery>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GetAllStreetcodesCatalogQueryValidator"/> class.
        /// </summary>
        public GetAllStreetcodesCatalogQueryValidator()
        {
            RuleLevelCascadeMode = CascadeMode.Stop;

            RuleFor(x => x.page)
                .GreaterThan(0)
                .WithMessage(ErrorMessages.PageNumberMustBePositive);

            RuleFor(x => x.count)
                .GreaterThan(0)
                .WithMessage(ErrorMessages.PageSizeMustBePositive);
        }
    }
}