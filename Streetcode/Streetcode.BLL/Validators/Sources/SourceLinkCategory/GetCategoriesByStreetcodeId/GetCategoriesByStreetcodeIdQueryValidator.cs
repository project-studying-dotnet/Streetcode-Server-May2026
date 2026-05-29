using Streetcode.BLL.MediatR.Sources.SourceLink.GetCategoriesByStreetcodeId;

namespace Streetcode.BLL.Validators.Sources.SourceLinkCategory.GetCategoriesByStreetcodeId
{
    public class GetCategoriesByStreetcodeIdQueryValidator : PositiveStreetcodeIdValidator<GetCategoriesByStreetcodeIdQuery>
    {
    }
}