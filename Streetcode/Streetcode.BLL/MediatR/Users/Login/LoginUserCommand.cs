using FluentResults;
using MediatR;
using Streetcode.BLL.DTO.Users;

namespace Streetcode.BLL.MediatR.Users.Login
{
    public record LoginUserCommand(UserLoginDTO LoginDto)
        : IRequest<Result<LoginResultDTO>>;
}
