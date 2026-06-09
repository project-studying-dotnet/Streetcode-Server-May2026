using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Mvc;
using Streetcode.BLL.MediatR.AdditionalContent.Coordinate.GetByStreetcodeId;

namespace Streetcode.WebApi.Controllers.AdditionalContent;

[ExcludeFromCodeCoverage]
public sealed class CoordinateController : BaseApiController
{
    [HttpGet("{streetcodeId:int}")]
    public async Task<IActionResult> GetByStreetcodeId([FromRoute] int streetcodeId, CancellationToken cancellationToken = default)
    {
        return base.HandleResult(
            await base.Mediator.Send(new GetCoordinatesByStreetcodeIdQuery(streetcodeId), cancellationToken)
        );
    }
}