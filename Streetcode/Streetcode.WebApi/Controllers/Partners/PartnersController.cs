using System.Diagnostics.CodeAnalysis;
using Streetcode.DAL.Enums;
using Microsoft.AspNetCore.Mvc;
using Streetcode.BLL.DTO.Partners;
using Streetcode.WebApi.Attributes;
using Streetcode.BLL.MediatR.Partners.Create;
using Streetcode.BLL.MediatR.Partners.GetAll;
using Streetcode.BLL.MediatR.Partners.GetById;
using Streetcode.BLL.MediatR.Partners.GetByStreetcodeId;
using Streetcode.BLL.MediatR.Partners.GetAllPartnerShort;

namespace Streetcode.WebApi.Controllers.Partners;

[ExcludeFromCodeCoverage]
public sealed class PartnersController : BaseApiController
{
    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken = default)
    {
        return base.HandleResult(
            await base.Mediator.Send(new GetAllPartnersQuery(), cancellationToken)
        );
    }

    [HttpGet]
    public async Task<IActionResult> GetAllShort(CancellationToken cancellationToken = default)
    {
        return base.HandleResult(
            await base.Mediator.Send(new GetAllPartnersShortQuery(), cancellationToken)
        );
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById([FromRoute] int id, CancellationToken cancellationToken = default)
    {
        return base.HandleResult(
            await base.Mediator.Send(new GetPartnerByIdQuery(id), cancellationToken)
        );
    }

    [HttpGet("{streetcodeId:int}")]
    public async Task<IActionResult> GetByStreetcodeId([FromRoute] int streetcodeId, CancellationToken cancellationToken = default)
    {
        return base.HandleResult(
            await base.Mediator.Send(new GetPartnersByStreetcodeIdQuery(streetcodeId), cancellationToken)
        );
    }

    [HttpPost]
    [AuthorizeRoles(UserRole.MainAdministrator)]
    public async Task<IActionResult> Create([FromBody] CreatePartnerDTO partner, CancellationToken cancellationToken = default)
    {
        return base.HandleResult(
            await base.Mediator.Send(new CreatePartnerQuery(partner), cancellationToken)
        );
    }

    [HttpPut]
    [AuthorizeRoles(UserRole.MainAdministrator)]
    public async Task<IActionResult> Update([FromBody] CreatePartnerDTO partner, CancellationToken cancellationToken = default)
    {
        return base.HandleResult(
            await base.Mediator.Send(new BLL.MediatR.Partners.Update.UpdatePartnerQuery(partner), cancellationToken)
        );
    }

    [HttpDelete("{id:int}")]
    [AuthorizeRoles(UserRole.MainAdministrator)]
    public async Task<IActionResult> Delete([FromRoute] int id, CancellationToken cancellationToken = default)
    {
        return base.HandleResult(
            await base.Mediator.Send(new BLL.MediatR.Partners.Delete.DeletePartnerQuery(id), cancellationToken)
        );
    }
}