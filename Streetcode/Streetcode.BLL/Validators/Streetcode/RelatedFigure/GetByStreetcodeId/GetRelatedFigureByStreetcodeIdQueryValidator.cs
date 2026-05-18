using Streetcode.BLL.MediatR.Streetcode.RelatedFigure.GetByStreetcodeId;

namespace Streetcode.BLL.Validators.Streetcode.RelatedFigure.GetByStreetcodeId
{
    /// <summary>
    /// Validator for GetRelatedFigureByStreetcodeIdQuery.
    /// </summary>
    public class GetRelatedFigureByStreetcodeIdQueryValidator
        : PositiveStreetcodeIdValidator<GetRelatedFigureByStreetcodeIdQuery>
    {
    }
}