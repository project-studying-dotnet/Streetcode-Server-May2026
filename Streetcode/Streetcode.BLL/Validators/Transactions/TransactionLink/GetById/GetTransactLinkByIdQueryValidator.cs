using Streetcode.BLL.MediatR.Transactions.TransactionLink.GetById;

namespace Streetcode.BLL.Validators.Transactions.TransactionLink.GetById
{
    /// <summary>
    /// Validator for <see cref="GetTransactLinkByIdQuery"/>.
    /// </summary>
    public class GetTransactLinkByIdQueryValidator : PositiveIdValidator<GetTransactLinkByIdQuery>
    {
    }
}
