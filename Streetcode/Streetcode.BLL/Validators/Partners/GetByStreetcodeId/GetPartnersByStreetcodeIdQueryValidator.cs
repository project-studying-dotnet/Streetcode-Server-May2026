using Streetcode.BLL.MediatR.Partners.GetByStreetcodeId;

namespace Streetcode.BLL.Validators.Partners.GetByStreetcodeId
{
    public class GetPartnersByStreetcodeIdQueryValidator : PositiveStreetcodeIdValidator<GetPartnersByStreetcodeIdQuery>
    {
    }
}