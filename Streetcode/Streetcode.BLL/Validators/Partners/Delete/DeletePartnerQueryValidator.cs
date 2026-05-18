using Streetcode.BLL.MediatR.Partners.Delete;

namespace Streetcode.BLL.Validators.Partners.Delete
{
    /// <summary>
    /// Validator for DeletePartnerQuery.
    /// </summary>
    public class DeletePartnerQueryValidator : PositiveIdValidator<DeletePartnerQuery>
    {
    }
}
