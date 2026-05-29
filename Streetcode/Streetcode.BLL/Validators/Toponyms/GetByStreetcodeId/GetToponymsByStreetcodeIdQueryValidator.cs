using Streetcode.BLL.MediatR.Toponyms.GetByStreetcodeId;

namespace Streetcode.BLL.Validators.Toponyms.GetByStreetcodeId
{
    public class GetToponymsByStreetcodeIdQueryValidator
        : PositiveStreetcodeIdValidator<GetToponymsByStreetcodeIdQuery>
    {
    }
}
