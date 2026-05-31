using FluentResults;
using MediatR;
using Streetcode.BLL.DTO.Users;

namespace Streetcode.BLL.MediatR.Users.RefreshToken
{
    public record RefreshTokenCommand(RefreshTokenRequestDto RefreshTokenRequest)
        : IRequest<Result<LoginResultDto>>;
}