using FluentResults;
using MediatR;

namespace Streetcode.BLL.MediatR.Users.Logout
{
    public record LogoutUserCommand(int UserId) : IRequest<Result<Unit>>;
}
