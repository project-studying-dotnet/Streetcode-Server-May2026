using Streetcode.DAL.Enums;
using Microsoft.AspNetCore.Mvc;
using Streetcode.WebApi.Attributes;
using Streetcode.BLL.DTO.Streetcode;
using Streetcode.BLL.DTO.AdditionalContent.Filter;
using Streetcode.BLL.MediatR.Streetcode.Streetcode.Create;
using Streetcode.BLL.MediatR.Streetcode.Streetcode.Update;
using Streetcode.BLL.MediatR.Streetcode.Streetcode.Delete;
using Streetcode.BLL.MediatR.Streetcode.Streetcode.GetAll;
using Streetcode.BLL.MediatR.Streetcode.Streetcode.GetById;
using Streetcode.BLL.MediatR.Streetcode.Streetcode.GetCount;
using Streetcode.BLL.MediatR.Streetcode.Streetcode.GetByFilter;
using Streetcode.BLL.MediatR.Streetcode.Streetcode.GetAllShort;
using Streetcode.BLL.MediatR.Streetcode.Streetcode.GetShortById;
using Streetcode.BLL.MediatR.Streetcode.Streetcode.GetAllCatalog;
using Streetcode.BLL.MediatR.Streetcode.Streetcode.GetByTransliterationUrl;
using Streetcode.BLL.MediatR.Streetcode.Streetcode.GetAllStreetcodesMainPage;

namespace Streetcode.WebApi.Controllers.Streetcode;

public sealed class StreetcodeController : BaseApiController
{
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] GetAllStreetcodesRequestDTO request, CancellationToken cancellationToken = default)
    {
        return base.HandleResult(
            await base.Mediator.Send(new GetAllStreetcodesQuery(request), cancellationToken)
        );
    }

    [HttpGet]
    public async Task<IActionResult> GetAllShort(CancellationToken cancellationToken = default)
    {
        return base.HandleResult(
            await base.Mediator.Send(new GetAllStreetcodesShortQuery(), cancellationToken)
        );
    }

    [HttpGet]
    public async Task<IActionResult> GetAllMainPage(CancellationToken cancellationToken = default)
    {
        return base.HandleResult(
            await base.Mediator.Send(new GetAllStreetcodesMainPageQuery(), cancellationToken)
        );
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetShortById(int id, CancellationToken cancellationToken = default)
    {
        return base.HandleResult(
            await base.Mediator.Send(new GetStreetcodeShortByIdQuery(id), cancellationToken)
        );
    }

    [HttpGet]
    public async Task<IActionResult> GetByFilter([FromQuery] StreetcodeFilterRequestDTO request, CancellationToken cancellationToken = default)
    {
        return base.HandleResult(
            await base.Mediator.Send(new GetStreetcodeByFilterQuery(request), cancellationToken)
        );
    }

    [HttpGet]
    public async Task<IActionResult> GetAllCatalog([FromQuery] int page, [FromQuery] int count, CancellationToken cancellationToken = default)
    {
        return base.HandleResult(
            await base.Mediator.Send(new GetAllStreetcodesCatalogQuery(page, count), cancellationToken)
        );
    }

    [HttpGet]
    public async Task<IActionResult> GetCount(CancellationToken cancellationToken = default)
    {
        return base.HandleResult(
            await base.Mediator.Send(new GetStreetcodesCountQuery(), cancellationToken)
        );
    }

    [HttpGet("{url}")]
    public async Task<IActionResult> GetByTransliterationUrl([FromRoute] string url, CancellationToken cancellationToken = default)
    {
        return base.HandleResult(
            await base.Mediator.Send(new GetStreetcodeByTransliterationUrlQuery(url), cancellationToken)
        );
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById([FromRoute] int id, CancellationToken cancellationToken = default)
    {
        return base.HandleResult(
            await base.Mediator.Send(new GetStreetcodeByIdQuery(id), cancellationToken)
        );
    }

    [HttpPost]
    [AuthorizeRoles(UserRole.MainAdministrator)]
    public async Task<IActionResult> Create([FromBody] StreetcodeDTO streetcode, CancellationToken cancellationToken = default)
    {
        return base.HandleResult(
            await base.Mediator.Send(new CreateStreetcodeCommand(streetcode), cancellationToken)
        );
    }

    [HttpPut]
    [AuthorizeRoles(UserRole.MainAdministrator)]
    public async Task<IActionResult> Update([FromRoute] int id, [FromBody] StreetcodeDTO streetcode, CancellationToken cancellationToken = default)
    {
        return base.HandleResult(
            await base.Mediator.Send(new UpdateStreetcodeCommand(streetcode), cancellationToken)
        );
    }

    [HttpDelete("{id:int}")]
    [AuthorizeRoles(UserRole.MainAdministrator)]
    public async Task<IActionResult> Delete([FromRoute] int id, CancellationToken cancellationToken = default)
    {
        return base.HandleResult(
            await base.Mediator.Send(new DeleteStreetcodeCommand(id), cancellationToken)
        );
    }
}