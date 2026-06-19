using FluentResults;
using MediatR;
using Streetcode.BLL.DTO.Users;

namespace Streetcode.BLL.MediatR.Users.ResetPassword
{
    public record ResetPasswordCommand(ResetPasswordDto ResetPasswordDTO) : IRequest<Result<Unit>>;
}
