using Streetcode.BLL.MediatR.Partners.GetById;

namespace Streetcode.BLL.Validators.Partners.GetById
{
    /// <summary>
    /// Validator for GetPartnerByIdQuery.
    /// </summary>
    public class GetPartnerByIdQueryValidator : PositiveIdValidator<GetPartnerByIdQuery>
    {
    }
}