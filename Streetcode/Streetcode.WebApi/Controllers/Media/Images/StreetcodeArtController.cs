using Microsoft.AspNetCore.Mvc;
using Streetcode.BLL.MediatR.Media.StreetcodeArt.GetByStreetcodeId;

namespace Streetcode.WebApi.Controllers.Media.Images;

public sealed class StreetcodeArtController : BaseApiController
{
    [HttpGet("{streetcodeId:int}")]
    public async Task<IActionResult> GetByStreetcodeId([FromRoute] int streetcodeId, CancellationToken cancellationToken = default)
    {
        return base.HandleResult(
            await base.Mediator.Send(new GetStreetcodeArtByStreetcodeIdQuery(streetcodeId), cancellationToken)
        );
    }
}