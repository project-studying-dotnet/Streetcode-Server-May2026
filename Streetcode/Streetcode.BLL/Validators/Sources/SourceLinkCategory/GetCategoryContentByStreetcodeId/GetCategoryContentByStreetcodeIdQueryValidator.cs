using FluentValidation;
using Streetcode.BLL.MediatR.Sources.SourceLinkCategory.GetCategoryContentByStreetcodeId;
using Streetcode.BLL.Resources;

namespace Streetcode.BLL.Validators.Sources.SourceLinkCategory.GetCategoryContentByStreetcodeId
{
    /// <summary>
    /// Validator for GetCategoryContentByStreetcodeIdQuery.
    /// </summary>
    public class GetCategoryContentByStreetcodeIdQueryValidator
        : AbstractValidator<GetCategoryContentByStreetcodeIdQuery>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GetCategoryContentByStreetcodeIdQueryValidator"/> class.
        /// </summary>
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