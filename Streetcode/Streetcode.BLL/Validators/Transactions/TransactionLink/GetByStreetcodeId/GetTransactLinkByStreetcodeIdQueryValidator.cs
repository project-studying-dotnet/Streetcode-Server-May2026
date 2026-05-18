using Streetcode.BLL.MediatR.Transactions.TransactionLink.GetByStreetcodeId;

namespace Streetcode.BLL.Validators.Transactions.TransactionLink.GetByStreetcodeId
{
    /// <summary>
    /// Validator for <see cref="GetTransactLinkByStreetcodeIdQuery"/>.
    /// </summary>
    public class GetTransactLinkByStreetcodeIdQueryValidator
        : PositiveStreetcodeIdValidator<GetTransactLinkByStreetcodeIdQuery>
    {
    }
}
