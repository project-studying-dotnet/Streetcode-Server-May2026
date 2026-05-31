using Streetcode.BLL.MediatR.Streetcode.RelatedFigure.GetByStreetcodeId;

namespace Streetcode.BLL.Validators.Streetcode.RelatedFigure.GetByStreetcodeId
{
    public class GetRelatedFigureByStreetcodeIdQueryValidator
        : PositiveStreetcodeIdValidator<GetRelatedFigureByStreetcodeIdQuery>
    {
    }
}