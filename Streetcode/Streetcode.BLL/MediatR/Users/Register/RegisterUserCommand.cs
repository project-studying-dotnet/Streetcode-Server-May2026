using FluentResults;
using MediatR;
using Streetcode.BLL.DTO.Users;

namespace Streetcode.BLL.MediatR.Users.Register
{
    public record RegisterUserCommand(UserRegisterDto registerRequest)
    : IRequest<Result<Unit>>;
}
