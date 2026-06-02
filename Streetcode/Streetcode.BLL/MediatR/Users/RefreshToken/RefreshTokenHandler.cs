using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Streetcode.BLL.DTO.Users;
using Streetcode.BLL.Extensions;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.Interfaces.Users;
using Streetcode.BLL.Settings;
using Streetcode.DAL.Entities.Users;

namespace Streetcode.BLL.MediatR.Users.RefreshToken
{
    public class RefreshTokenHandler : IRequestHandler<RefreshTokenCommand, Result<LoginResultDto>>
    {
        private readonly UserManager<User> _userManager;
        private readonly ILoggerService _logger;
        private readonly ITokenService _tokenService;
        private readonly JwtSettings _jwtSettings;

        public RefreshTokenHandler(
            UserManager<User> userManager,
            ILoggerService logger,
            ITokenService tokenService,
            JwtSettings jwtSettings)
        {
            _userManager = userManager;
            _logger = logger;
            _tokenService = tokenService;
            _jwtSettings = jwtSettings;
        }

        public async Task<Result<LoginResultDto>> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
        {
            var principal = _tokenService.GetPrincipalFromExpiredToken(request.RefreshTokenRequest.Token);

            var userId = principal.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId is null)
            {
                return Result.Fail<LoginResultDto>("Invalid token.");
            }

            var user = await _userManager.FindByIdAsync(userId);

            if (user is null ||
                user.RefreshToken != request.RefreshTokenRequest.RefreshToken ||
                user.RefreshTokenExpiryTime <= DateTime.UtcNow)
            {
                return Result.Fail<LoginResultDto>("Invalid refresh token.");
            }

            var jwtToken = _tokenService.GenerateJWTToken(user);
            var token = new JwtSecurityTokenHandler().WriteToken(jwtToken);

            var refreshToken = _tokenService.GenerateRefreshToken();

            user.EnsureSecurityStamp();
            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenLifetimeInDays);

            await _userManager.UpdateAsync(user);

            var loginResult = new LoginResultDto
            {
                User = new UserDto
                {
                    Id = user.Id,
                    Name = user.Name,
                    Surname = user.Surname,
                    Login = user.UserName ?? string.Empty,
                    Role = user.Role,
                },
                Token = token,
                RefreshToken = refreshToken,
                ExpireAt = jwtToken.ValidTo,
            };

            _logger.LogInformation($"Token refreshed for user {user.Id}");

            return Result.Ok(loginResult);
        }
    }
}