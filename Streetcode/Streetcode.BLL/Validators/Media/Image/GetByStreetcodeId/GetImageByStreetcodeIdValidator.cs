using Streetcode.BLL.MediatR.Media.Image.GetByStreetcodeId;

namespace Streetcode.BLL.Validators.Media.Image.GetByStreetcodeId
{
    /// <summary>
    /// Validator for GetImageByStreetcodeIdQuery.
    /// </summary>
    public class GetImageByStreetcodeIdValidator
    : PositiveStreetcodeIdValidator<GetImageByStreetcodeIdQuery>
    {
    }
}
