using FluentResults;
using Microsoft.AspNetCore.Mvc;
using Streetcode.BLL.DTO.Streetcode.TextContent.Text;
using Streetcode.BLL.MediatR.Streetcode.Text.Create;
using Streetcode.BLL.MediatR.Streetcode.Text.Delete;
using Streetcode.BLL.MediatR.Streetcode.Text.GetAll;
using Streetcode.BLL.MediatR.Streetcode.Text.GetById;
using Streetcode.BLL.MediatR.Streetcode.Text.GetByStreetcodeId;
using Streetcode.BLL.MediatR.Streetcode.Text.GetParsed;
using Streetcode.BLL.MediatR.Streetcode.Text.Update;

namespace Streetcode.WebApi.Controllers.Streetcode.TextContent;

public class TextController : BaseApiController
{
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        return HandleResult(await Mediator.Send(new GetAllTextsQuery()));
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById([FromRoute] int id)
    {
        return HandleResult(await Mediator.Send(new GetTextByIdQuery(id)));
    }

    [HttpGet("{streetcodeId:int}")]
    public async Task<IActionResult> GetByStreetcodeId([FromRoute] int streetcodeId)
    {
        return HandleResult(await Mediator.Send(new GetTextByStreetcodeIdQuery(streetcodeId)));
    }

    [HttpGet]
    public async Task<IActionResult> GetParsedText([FromQuery] string text)
    {
        return HandleResult(await Mediator.Send(new GetParsedTextForAdminPreviewCommand(text)));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] TextCreateDTO createTextRequest)
    {
        return HandleResult(await Mediator.Send(new CreateTextCommand(createTextRequest)));
    }

    [HttpPut]
    public async Task<IActionResult> Update([FromRoute] int id, [FromBody] TextUpdateDTO updateTextRequest)
    {
        return HandleResult(await Mediator.Send(new UpdateTextCommand(updateTextRequest)));
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete([FromRoute] int id)
    {
        return HandleResult(await Mediator.Send(new DeleteTextCommand(id)));
    }
}