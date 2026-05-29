using FluentValidation;
using Streetcode.BLL.MediatR.Streetcode.Streetcode.GetAllCatalog;
using Streetcode.BLL.Resources;

namespace Streetcode.BLL.Validators.Streetcode.Streetcode.GetAllCatalog
{
    public class GetAllStreetcodesCatalogQueryValidator
        : AbstractValidator<GetAllStreetcodesCatalogQuery>
    {
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