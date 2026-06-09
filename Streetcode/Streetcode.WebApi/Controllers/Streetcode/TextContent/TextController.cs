using Streetcode.DAL.Enums;
using Microsoft.AspNetCore.Mvc;
using Streetcode.WebApi.Attributes;
using Streetcode.BLL.MediatR.Streetcode.Text.Update;
using Streetcode.BLL.MediatR.Streetcode.Text.Create;
using Streetcode.BLL.MediatR.Streetcode.Text.Delete;
using Streetcode.BLL.MediatR.Streetcode.Text.GetAll;
using Streetcode.BLL.MediatR.Streetcode.Text.GetById;
using Streetcode.BLL.DTO.Streetcode.TextContent.Text;
using Streetcode.BLL.MediatR.Streetcode.Text.GetParsed;
using Streetcode.BLL.MediatR.Streetcode.Text.GetByStreetcodeId;

namespace Streetcode.WebApi.Controllers.Streetcode.TextContent;

public sealed class TextController : BaseApiController
{
    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken = default)
    {
        return base.HandleResult(
            await base.Mediator.Send(new GetAllTextsQuery(), cancellationToken)
        );
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById([FromRoute] int id, CancellationToken cancellationToken = default)
    {
        return base.HandleResult(
            await base.Mediator.Send(new GetTextByIdQuery(id), cancellationToken)
        );
    }

    [HttpGet("{streetcodeId:int}")]
    public async Task<IActionResult> GetByStreetcodeId([FromRoute] int streetcodeId, CancellationToken cancellationToken = default)
    {
        return base.HandleResult(
            await base.Mediator.Send(new GetTextByStreetcodeIdQuery(streetcodeId), cancellationToken)
        );
    }

    [HttpGet]
    public async Task<IActionResult> GetParsedText([FromQuery] string text, CancellationToken cancellationToken = default)
    {
        return base.HandleResult(
            await base.Mediator.Send(new GetParsedTextForAdminPreviewCommand(text), cancellationToken)
        );
    }

    [HttpPost]
    [AuthorizeRoles(UserRole.MainAdministrator)]
    public async Task<IActionResult> Create([FromBody] TextCreateDto createTextRequest, CancellationToken cancellationToken = default)
    {
        return base.HandleResult(
            await base.Mediator.Send(new CreateTextCommand(createTextRequest), cancellationToken)
        );
    }

    [HttpPut]
    [AuthorizeRoles(UserRole.MainAdministrator)]
    public async Task<IActionResult> Update([FromRoute] int id, [FromBody] TextUpdateDto updateTextRequest, CancellationToken cancellationToken = default)
    {
        updateTextRequest.Id = id;
        return base.HandleResult(
            await base.Mediator.Send(new UpdateTextCommand(updateTextRequest), cancellationToken)
        );
    }

    [HttpDelete("{id:int}")]
    [AuthorizeRoles(UserRole.MainAdministrator)]
    public async Task<IActionResult> Delete([FromRoute] int id, CancellationToken cancellationToken = default)
    {
        return base.HandleResult(
            await base.Mediator.Send(new DeleteTextCommand(id), cancellationToken)
        );
    }
}