using Microsoft.AspNetCore.Mvc;
using Streetcode.BLL.MediatR.Transactions.TransactionLink.GetAll;
using Streetcode.BLL.MediatR.Transactions.TransactionLink.GetById;
using Streetcode.BLL.MediatR.Transactions.TransactionLink.GetByStreetcodeId;

namespace Streetcode.WebApi.Controllers.Transactions;

public sealed class TransactLinksController : BaseApiController
{
    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken = default)
    {
        return base.HandleResult(
            await base.Mediator.Send(new GetAllTransactLinksQuery(), cancellationToken)
        );
    }

    [HttpGet("{streetcodeId:int}")]
    public async Task<IActionResult> GetByStreetcodeId([FromRoute] int streetcodeId, CancellationToken cancellationToken = default)
    {
        return base.HandleResult(
            await base.Mediator.Send(new GetTransactLinkByStreetcodeIdQuery(streetcodeId), cancellationToken)
        );
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById([FromRoute] int id, CancellationToken cancellationToken = default)
    {
        return base.HandleResult(
            await base.Mediator.Send(new GetTransactLinkByIdQuery(id), cancellationToken)
        );
    }
}