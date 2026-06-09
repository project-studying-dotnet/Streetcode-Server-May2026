using Streetcode.DAL.Enums;
using Streetcode.BLL.DTO.News;
using Microsoft.AspNetCore.Mvc;
using Streetcode.WebApi.Attributes;
using Streetcode.BLL.MediatR.Newss.Update;
using Streetcode.BLL.MediatR.Newss.Create;
using Streetcode.BLL.MediatR.Newss.Delete;
using Streetcode.BLL.MediatR.Newss.GetAll;
using Streetcode.BLL.MediatR.Newss.GetById;
using Streetcode.BLL.MediatR.Newss.GetByUrl;
using Streetcode.BLL.MediatR.Newss.SortedByDateTime;
using Streetcode.BLL.MediatR.Newss.GetNewsAndLinksByUrl;

namespace Streetcode.WebApi.Controllers.News;

public sealed class NewsController : BaseApiController
{
    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken = default)
    {
        return base.HandleResult(
            await base.Mediator.Send(new GetAllNewsQuery(), cancellationToken)
        );
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById([FromRoute] int id, CancellationToken cancellationToken = default)
    {
        return base.HandleResult(
            await base.Mediator.Send(new GetNewsByIdQuery(id), cancellationToken)
        );
    }

    [HttpGet("{url}")]
    public async Task<IActionResult> GetByUrl([FromRoute] string url, CancellationToken cancellationToken = default)
    {
        return base.HandleResult(
            await base.Mediator.Send(new GetNewsByUrlQuery(url), cancellationToken)
        );
    }

    [HttpGet("sorted")]
    public async Task<IActionResult> GetSortedByDateTime(CancellationToken cancellationToken = default)
    {
        return base.HandleResult(
            await base.Mediator.Send(new SortedByDateTimeQuery(), cancellationToken)
        );
    }

    [HttpGet("with-links")]
    public async Task<IActionResult> GetNewsAndLinksByUrl([FromQuery] string url, CancellationToken cancellationToken = default)
    {
        return base.HandleResult(
            await base.Mediator.Send(new GetNewsAndLinksByUrlQuery(url), cancellationToken)
        );
    }

    [HttpPost]
    [AuthorizeRoles(UserRole.Administrator)]
    public async Task<IActionResult> Create([FromBody] NewsDTO newsDto, CancellationToken cancellationToken = default)
    {
        return base.HandleResult(
            await base.Mediator.Send(new CreateNewsCommand(newsDto), cancellationToken)
        );
    }

    [HttpPut("{id:int}")]
    [AuthorizeRoles(UserRole.Administrator)]
    public async Task<IActionResult> Update([FromRoute] int id, [FromBody] NewsDTO newsDto, CancellationToken cancellationToken = default)
    {
        newsDto.Id = id;
        return base.HandleResult(
            await base.Mediator.Send(new UpdateNewsCommand(newsDto), cancellationToken)
        );
    }

    [HttpDelete("{id:int}")]
    [AuthorizeRoles(UserRole.Administrator)]
    public async Task<IActionResult> Delete([FromRoute] int id, CancellationToken cancellationToken = default)
    {
        return base.HandleResult(
            await base.Mediator.Send(new DeleteNewsCommand(id), cancellationToken)
        );
    }
}