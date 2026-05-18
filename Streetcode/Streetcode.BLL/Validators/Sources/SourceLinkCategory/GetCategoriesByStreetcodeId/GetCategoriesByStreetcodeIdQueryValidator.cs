using Streetcode.BLL.MediatR.Sources.SourceLink.GetCategoriesByStreetcodeId;

namespace Streetcode.BLL.Validators.Sources.SourceLinkCategory.GetCategoriesByStreetcodeId
{
    /// <summary>
    /// Validator for GetCategoriesByStreetcodeIdQuery.
    /// </summary>
    public class GetCategoriesByStreetcodeIdQueryValidator : PositiveStreetcodeIdValidator<GetCategoriesByStreetcodeIdQuery>
    {
    }
}