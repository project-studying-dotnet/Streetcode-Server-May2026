using FluentResults;
using MediatR;
using Streetcode.BLL.DTO.Users;

namespace Streetcode.BLL.MediatR.Users.Login
{
    public record LoginUserCommand(UserLoginDto loginRequest)
        : IRequest<Result<LoginResultDto>>;
}
