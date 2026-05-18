using Streetcode.BLL.MediatR.AdditionalContent.Coordinate.GetByStreetcodeId;
using Streetcode.BLL.MediatR.AdditionalContent.Tag.GetByStreetcodeId;

namespace Streetcode.BLL.Validators.AdditionalContent.Tag.GetByStreetcodeId
{
    /// <summary>
    /// Validator for GetTagByStreetcodeIdValidator.
    /// </summary>
    public class GetTagByStreetcodeIdValidator : PositiveStreetcodeIdValidator<GetTagByStreetcodeIdQuery>
    {
    }
}
