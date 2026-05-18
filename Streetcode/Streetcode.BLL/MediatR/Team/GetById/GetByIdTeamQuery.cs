using FluentResults;
using MediatR;
using Streetcode.BLL.DTO.Team;
using Streetcode.BLL.MediatR.Interface;

namespace Streetcode.BLL.MediatR.Team.GetById
{
    public record GetByIdTeamQuery(int Id) : IRequest<Result<TeamMemberDTO>>, IHasId;
}
