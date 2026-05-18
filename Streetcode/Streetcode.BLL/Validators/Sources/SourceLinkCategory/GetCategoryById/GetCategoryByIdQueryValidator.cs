using Streetcode.BLL.MediatR.Sources.SourceLink.GetCategoryById;

namespace Streetcode.BLL.Validators.Sources.SourceLinkCategory.GetCategoryById
{
    /// <summary>
    /// Validator for GetCategoryByIdQuery.
    /// </summary>
    public class GetCategoryByIdQueryValidator : PositiveIdValidator<GetCategoryByIdQuery>
    {
    }
}