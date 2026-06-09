using Microsoft.AspNetCore.Mvc;
using Streetcode.BLL.DTO.Payment;
using Streetcode.BLL.MediatR.Payment;

namespace Streetcode.WebApi.Controllers.Payment;

public sealed class PaymentController : BaseApiController
{
    [HttpPost]
    public async Task<IActionResult> CreateInvoice([FromBody] PaymentDTO payment, CancellationToken cancellationToken = default)
    {
        return base.HandleResult(
            await base.Mediator.Send(new CreateInvoiceCommand(payment), cancellationToken)
        );
    }
}