using FluentResults;
using MediatR;
using Streetcode.Auth.Models.DTO;

namespace Streetcode.Auth.MediatR.Users.Register
{
    public record RegisterUserCommand(UserRegisterDto registerRequest)
    : IRequest<Result<Unit>>;
}
