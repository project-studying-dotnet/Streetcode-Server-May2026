using FluentResults;
using MediatR;
using Streetcode.BLL.DTO.Users;

namespace Streetcode.BLL.MediatR.Users.LoginGoogle
{
    public record GoogleLoginCommand(GoogleLoginRequestDto googleLoginRequest)
      : IRequest<Result<LoginResultDto>>;
}
