using FluentValidation;
using Streetcode.BLL.MediatR.Sources.SourceLinkCategory.GetCategoryContentByStreetcodeId;

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
            RuleFor(x => x.streetcodeId)
                .GreaterThan(0)
                .WithMessage("The streetcodeId must be positive.");

            RuleFor(x => x.categoryId)
                .GreaterThan(0)
                .WithMessage("The categoryId must be positive.");
        }
    }
}