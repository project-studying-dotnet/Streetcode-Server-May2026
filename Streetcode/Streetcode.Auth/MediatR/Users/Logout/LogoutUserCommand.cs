using FluentResults;
using MediatR;

namespace Streetcode.Auth.MediatR.Users.Logout
{
    public record LogoutUserCommand(string RefreshToken) : IRequest<Result<Unit>>;
}
