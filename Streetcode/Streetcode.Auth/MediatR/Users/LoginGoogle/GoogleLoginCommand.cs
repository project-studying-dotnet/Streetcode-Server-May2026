using FluentResults;
using MediatR;
using Streetcode.Auth.Models.DTO;

namespace Streetcode.Auth.MediatR.Users.LoginGoogle
{
    public record GoogleLoginCommand(GoogleLoginRequestDto googleLoginRequest)
      : IRequest<Result<AuthResponseDto>>;
}
