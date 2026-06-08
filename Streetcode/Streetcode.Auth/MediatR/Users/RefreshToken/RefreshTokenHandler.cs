using AutoMapper;
using FluentResults;
using MediatR;
using Microsoft.IdentityModel.Tokens;
using Streetcode.Auth.Models.DTO;
using Streetcode.Auth.Services.Interfaces.Users;
using System.IdentityModel.Tokens.Jwt;

namespace Streetcode.Auth.MediatR.Users.RefreshToken
{
    public class RefreshTokenHandler
        : IRequestHandler<RefreshTokenCommand, Result<LoginResultDto>>
    {
        private readonly IRefreshTokenService _refresh;
        private readonly IJwtTokenService _jwt;
        private readonly IMapper _mapper;

        public RefreshTokenHandler(
            IRefreshTokenService refresh,
            IJwtTokenService jwt,
            IMapper mapper)
        {
            _refresh = refresh;
            _jwt = jwt;
            _mapper = mapper;
        }

        public async Task<Result<LoginResultDto>> Handle(
            RefreshTokenCommand request,
            CancellationToken cancellationToken)
        {
            try
            {
                var (user, newRefresh) =
                    await _refresh.RefreshAsync(request.RefreshTokenRequest.RefreshToken);

                var jwt = _jwt.GenerateToken(user);
                var accessToken = new JwtSecurityTokenHandler().WriteToken(jwt);

                return Result.Ok(new LoginResultDto
                {
                    User = _mapper.Map<UserDto>(user),
                    Token = accessToken,
                    RefreshToken = newRefresh,
                    ExpireAt = jwt.ValidTo
                });
            }
            catch (SecurityTokenException ex)
            {
                return Result.Fail<LoginResultDto>(ex.Message);
            }
        }
    }
}