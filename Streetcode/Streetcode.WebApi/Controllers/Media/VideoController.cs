using Microsoft.AspNetCore.Mvc;
using Streetcode.BLL.DTO.Media;
using Streetcode.BLL.DTO.Media.Video;
using Streetcode.BLL.MediatR.Media.Video.Create;
using Streetcode.BLL.MediatR.Media.Video.Delete;
using Streetcode.BLL.MediatR.Media.Video.GetAll;
using Streetcode.BLL.MediatR.Media.Video.GetById;
using Streetcode.BLL.MediatR.Media.Video.GetByStreetcodeId;
using Streetcode.DAL.Enums;
using Streetcode.WebApi.Attributes;

namespace Streetcode.WebApi.Controllers.Media;

public class VideoController : BaseApiController
{
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        return HandleResult(await Mediator.Send(new GetAllVideosQuery()));
    }

    [HttpGet("{streetcodeId:int}")]
    public async Task<IActionResult> GetByStreetcodeId([FromRoute] int streetcodeId)
    {
        return HandleResult(await Mediator.Send(new GetVideoByStreetcodeIdQuery(streetcodeId)));
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById([FromRoute] int id)
    {
        return HandleResult(await Mediator.Send(new GetVideoByIdQuery(id)));
    }

    [AuthorizeRoles(UserRole.MainAdministrator)]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] VideoCreateDto createVideoRequest)
    {
        return HandleResult(await Mediator.Send(new CreateVideoCommand(createVideoRequest)));
    }

    [AuthorizeRoles(UserRole.MainAdministrator)]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete([FromRoute] int id)
    {
        return HandleResult(await Mediator.Send(new DeleteVideoCommand(id)));
    }
}
