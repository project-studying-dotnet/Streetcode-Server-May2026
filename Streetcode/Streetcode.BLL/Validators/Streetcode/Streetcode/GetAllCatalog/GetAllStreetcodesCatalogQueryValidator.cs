using FluentValidation;
using Streetcode.BLL.MediatR.Streetcode.Streetcode.GetAllCatalog;

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
            RuleFor(x => x.page)
                .GreaterThan(0)
                .WithMessage("The page number must be greater than 0.");

            RuleFor(x => x.count)
                .GreaterThan(0)
                .WithMessage("The page size (count) must be greater than 0.");
        }
    }
}