using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Mvc;
using Streetcode.BLL.MediatR.AdditionalContent.Tag.GetAll;
using Streetcode.BLL.MediatR.AdditionalContent.Tag.GetById;
using Streetcode.BLL.MediatR.AdditionalContent.Tag.GetByStreetcodeId;

namespace Streetcode.WebApi.Controllers.AdditionalContent;

[ExcludeFromCodeCoverage]
public sealed class TagController : BaseApiController
{
    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken = default)
    {
        return base.HandleResult(
            await base.Mediator.Send(new GetAllTagsQuery(), cancellationToken)
        );
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById([FromRoute] int id, CancellationToken cancellationToken = default)
    {
        return base.HandleResult(
            await base.Mediator.Send(new GetTagByIdQuery(id), cancellationToken)
        );
    }

    [HttpGet("{streetcodeId:int}")]
    public async Task<IActionResult> GetByStreetcodeId([FromRoute] int streetcodeId, CancellationToken cancellationToken = default)
    {
        return base.HandleResult(
            await base.Mediator.Send(new GetTagByStreetcodeIdQuery(streetcodeId), cancellationToken)
        );
    }

    [HttpGet("{title}")]
    public async Task<IActionResult> GetTagByTitle([FromRoute] string title, CancellationToken cancellationToken = default)
    {
        return base.HandleResult(
            await base.Mediator.Send(new GetTagByTitleQuery(title), cancellationToken)
        );
    }
}