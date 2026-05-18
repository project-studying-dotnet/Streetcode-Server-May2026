using Streetcode.BLL.MediatR.Partners.GetByStreetcodeId;

namespace Streetcode.BLL.Validators.Partners.GetByStreetcodeId
{
    /// <summary>
    /// Validator for GetPartnersByStreetcodeIdQuery.
    /// </summary>
    public class GetPartnersByStreetcodeIdQueryValidator : PositiveStreetcodeIdValidator<GetPartnersByStreetcodeIdQuery>
    {
    }
}