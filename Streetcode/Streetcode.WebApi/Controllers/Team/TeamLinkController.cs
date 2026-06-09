using Streetcode.DAL.Enums;
using Streetcode.BLL.DTO.Team;
using Microsoft.AspNetCore.Mvc;
using Streetcode.WebApi.Attributes;
using Streetcode.BLL.MediatR.Team.TeamMembersLinks.Create;
using Streetcode.BLL.MediatR.Team.TeamMembersLinks.GetAll;

namespace Streetcode.WebApi.Controllers.Team;

public sealed class TeamLinkController : BaseApiController
{
    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken = default)
    {
        return base.HandleResult(
            await base.Mediator.Send(new GetAllTeamLinkQuery(), cancellationToken)
        );
    }

    [HttpPost]
    [AuthorizeRoles(UserRole.MainAdministrator)]
    public async Task<IActionResult> Create([FromBody] TeamMemberLinkDTO teamMemberLink, CancellationToken cancellationToken = default)
    {
        return base.HandleResult(
            await base.Mediator.Send(new CreateTeamLinkQuery(teamMemberLink), cancellationToken)
        );
    }
}