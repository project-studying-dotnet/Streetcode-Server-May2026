using Microsoft.AspNetCore.Mvc;
using Streetcode.BLL.MediatR.Streetcode.RelatedFigure.Create;
using Streetcode.BLL.MediatR.Streetcode.RelatedFigure.Delete;
using Streetcode.BLL.MediatR.Streetcode.RelatedFigure.GetByTagId;
using Streetcode.BLL.MediatR.Streetcode.RelatedFigure.GetByStreetcodeId;
using Streetcode.DAL.Enums;
using Streetcode.WebApi.Attributes;

namespace Streetcode.WebApi.Controllers.Streetcode;

public sealed class RelatedFigureController : BaseApiController
{
    [HttpGet("{streetcodeId:int}")]
    public async Task<IActionResult> GetByStreetcodeId([FromRoute] int streetcodeId, CancellationToken cancellationToken = default)
    {
        return base.HandleResult(
            await base.Mediator.Send(new GetRelatedFigureByStreetcodeIdQuery(streetcodeId), cancellationToken)
        );
    }

    [HttpGet("{tagId:int}")]
    public async Task<IActionResult> GetByTagId([FromRoute] int tagId, CancellationToken cancellationToken = default)
    {
        return base.HandleResult(
            await base.Mediator.Send(new GetRelatedFiguresByTagIdQuery(tagId), cancellationToken)
        );
    }

    [AuthorizeRoles(UserRole.MainAdministrator)]
    [HttpPost("{ObserverId:int}&{TargetId:int}")]
    public async Task<IActionResult> Create([FromRoute] int ObserverId, int TargetId, CancellationToken cancellationToken = default)
    {
        return base.HandleResult(
            await base.Mediator.Send(new CreateRelatedFigureCommand(ObserverId, TargetId), cancellationToken)
        );
    }

    [AuthorizeRoles(UserRole.MainAdministrator)]
    [HttpDelete("{ObserverId:int}&{TargetId:int}")]
    public async Task<IActionResult> Delete([FromRoute] int ObserverId, int TargetId, CancellationToken cancellationToken = default)
    {
        return base.HandleResult(
            await base.Mediator.Send(new DeleteRelatedFigureCommand(ObserverId, TargetId), cancellationToken)
        );
    }
}