using Streetcode.BLL.MediatR.Streetcode.Fact.GetByStreetcodeId;

namespace Streetcode.BLL.Validators.Streetcode.Fact.GetByStreetcodeId
{
    /// <summary>
    /// Validator for GetFactByStreetcodeIdQuery.
    /// </summary>
    public class GetFactByStreetcodeIdQueryValidator : PositiveStreetcodeIdValidator<GetFactByStreetcodeIdQuery>
    {
    }
}