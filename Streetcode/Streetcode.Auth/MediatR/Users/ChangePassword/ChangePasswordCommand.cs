using FluentResults;
using MediatR;
using Streetcode.Auth.Models.DTO.Users;

namespace Streetcode.Auth.Models.MediatR.Users.ChangePassword
{
    public record ChangePasswordCommand(int UserId, ChangePasswordDto ChangePasswordRequest)
     : IRequest<Result<Unit>>;
}
