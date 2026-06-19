using FluentResults;
using MediatR;
using Streetcode.BLL.DTO.Users;

namespace Streetcode.BLL.MediatR.Users.ForgotPassword
{
    public record ForgotPasswordCommand(ForgotPasswordDto ForgotPasswordDto) : IRequest<Result<Unit>>;
}