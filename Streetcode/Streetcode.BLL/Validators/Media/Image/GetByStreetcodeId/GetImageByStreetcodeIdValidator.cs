using Streetcode.BLL.MediatR.Media.Image.GetByStreetcodeId;

namespace Streetcode.BLL.Validators.Media.Image.GetByStreetcodeId
{
    public class GetImageByStreetcodeIdValidator
    : PositiveStreetcodeIdValidator<GetImageByStreetcodeIdQuery>
    {
    }
}
