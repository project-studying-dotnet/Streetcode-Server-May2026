using FluentResults;
using MediatR;
using Streetcode.Auth.Models.DTO;

namespace Streetcode.Auth.MediatR.Users.RefreshToken
{
    public record RefreshTokenCommand(RefreshTokenRequestDto RefreshTokenRequest)
        : IRequest<Result<AuthResponseDto>>;
}