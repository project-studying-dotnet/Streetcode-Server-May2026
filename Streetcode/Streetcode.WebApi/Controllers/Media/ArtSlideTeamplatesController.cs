using Microsoft.AspNetCore.Mvc;
using Streetcode.BLL.MediatR.Media.ArtSlideTeamplates.GetAll;

namespace Streetcode.WebApi.Controllers.Media
{
    [ApiController]
    [Route("api/[controller]")]
    public class ArtSlideTeamplatesController : BaseApiController
    {
        [HttpGet]
        public async Task<IActionResult> GetAll(CancellationToken cancellationToken = default)
        {
            return base.HandleResult(
               await base.Mediator.Send(new GetAllArtSlideTemplatesQuery(), cancellationToken)
            );
        }
    }
}