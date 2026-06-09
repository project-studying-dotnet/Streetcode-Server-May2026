using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Mvc;
using Streetcode.BLL.DTO.Email;
using Streetcode.BLL.MediatR.Email;

namespace Streetcode.WebApi.Controllers.Email;

[ExcludeFromCodeCoverage]
public sealed class EmailController : BaseApiController
{
    [HttpPost]
    public async Task<IActionResult> Send([FromBody] EmailDTO email, CancellationToken cancellationToken = default)
    {
        return base.HandleResult(
            await base.Mediator.Send(new SendEmailCommand(email), cancellationToken)
        );
    }
}