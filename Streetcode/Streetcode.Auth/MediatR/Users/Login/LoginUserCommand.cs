using FluentResults;
using MediatR;
using Streetcode.Auth.Models.DTO;


namespace Streetcode.Auth.MediatR.Users.Login
{
    public record LoginUserCommand(UserLoginDto loginRequest)
        : IRequest<Result<AuthResponseDto>>;
}
