using FluentValidation;
using Streetcode.BLL.MediatR.Sources.SourceLinkCategory.GetCategoryContentByStreetcodeId;
using Streetcode.BLL.Resources;

namespace Streetcode.BLL.Validators.Sources.SourceLinkCategory.GetCategoryContentByStreetcodeId
{
    public class GetCategoryContentByStreetcodeIdQueryValidator
        : AbstractValidator<GetCategoryContentByStreetcodeIdQuery>
    {
        public GetCategoryContentByStreetcodeIdQueryValidator()
        {
            RuleLevelCascadeMode = CascadeMode.Stop;

            RuleFor(x => x.streetcodeId)
                .GreaterThan(0)
                .WithMessage(ErrorMessages.StreetcodeIdMustBePositive);

            RuleFor(x => x.categoryId)
                .GreaterThan(0)
                .WithMessage(ErrorMessages.CategoryIdMustBePositive);
        }
    }
}