using FluentResults;
using MediatR;
using Streetcode.BLL.DTO.Users;

namespace Streetcode.BLL.MediatR.Users.ChangePassword
{
    public record ChangePasswordCommand(int UserId, ChangePasswordDto ChangePasswordRequest)
     : IRequest<Result<Unit>>;
}
