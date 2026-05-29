using Streetcode.BLL.MediatR.Transactions.TransactionLink.GetByStreetcodeId;

namespace Streetcode.BLL.Validators.Transactions.TransactionLink.GetByStreetcodeId
{
    public class GetTransactLinkByStreetcodeIdQueryValidator
        : PositiveStreetcodeIdValidator<GetTransactLinkByStreetcodeIdQuery>
    {
    }
}
