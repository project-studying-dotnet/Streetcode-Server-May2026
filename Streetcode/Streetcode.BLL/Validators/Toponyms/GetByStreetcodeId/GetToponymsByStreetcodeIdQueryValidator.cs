using Streetcode.BLL.MediatR.Toponyms.GetByStreetcodeId;

namespace Streetcode.BLL.Validators.Toponyms.GetByStreetcodeId
{
    /// <summary>
    /// Validator for <see cref="GetToponymsByStreetcodeIdQuery"/>.
    /// </summary>
    public class GetToponymsByStreetcodeIdQueryValidator
        : PositiveStreetcodeIdValidator<GetToponymsByStreetcodeIdQuery>
    {
    }
}
