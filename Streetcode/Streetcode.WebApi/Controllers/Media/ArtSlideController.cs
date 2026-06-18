using Microsoft.AspNetCore.Mvc;
using Streetcode.BLL.DTO.Media.ArtSlides;
using Streetcode.BLL.MediatR.Media.ArtSlide.Create;
using Streetcode.BLL.MediatR.Media.ArtSlide.CreateAll;
using Streetcode.BLL.MediatR.Media.ArtSlide.Delete;
using Streetcode.BLL.MediatR.Media.ArtSlide.GetAllByStreetcodeId;
using Streetcode.BLL.MediatR.Media.ArtSlide.Update;
using Streetcode.DAL.Enums;
using Streetcode.WebApi.Attributes;

namespace Streetcode.WebApi.Controllers.Media
{
    [ApiController]
    [Route("api/[controller]")]
    public sealed class ArtSlideController : BaseApiController
    {
        [HttpGet("{streetcodeId:int}")]
        public async Task<IActionResult> GetAllByStreetcodeId([FromRoute] int streetcodeId, CancellationToken cancellationToken = default)
        {
            return base.HandleResult(
                await base.Mediator.Send(new GetAllArtSlidesByStreetcodeIdQuery(streetcodeId), cancellationToken)
            );
        }

        [HttpPost]
        [AuthorizeRoles(UserRole.MainAdministrator)]
        public async Task<IActionResult> Create([FromBody] CreateStreetcodeArtSlideDto dto, CancellationToken cancellationToken = default)
        {
            return base.HandleResult(
                await base.Mediator.Send(new CreateArtSlideCommand(dto), cancellationToken)
            );
        }

        [HttpPost("CreateAll")]
        [AuthorizeRoles(UserRole.MainAdministrator)]
        public async Task<IActionResult> CreateAll([FromBody] List<CreateStreetcodeArtSlideDto> dtos, CancellationToken cancellationToken = default)
        {
            return base.HandleResult(
                await base.Mediator.Send(new CreateAllArtSlidesCommand(dtos), cancellationToken)
            );
        }

        [HttpPut]
        [AuthorizeRoles(UserRole.MainAdministrator)]
        public async Task<IActionResult> Update([FromBody] UpdateArtSlideDto dto, CancellationToken cancellationToken = default)
        {
            return base.HandleResult(
                await base.Mediator.Send(new UpdateArtSlideCommand(dto), cancellationToken)
            );
        }

        [HttpDelete("{id:int}")]
        [AuthorizeRoles(UserRole.MainAdministrator)]
        public async Task<IActionResult> Delete([FromRoute] int id, CancellationToken cancellationToken = default)
        {
            return base.HandleResult(
                await base.Mediator.Send(new DeleteArtSlideCommand(id), cancellationToken)
            );
        }
    }
}