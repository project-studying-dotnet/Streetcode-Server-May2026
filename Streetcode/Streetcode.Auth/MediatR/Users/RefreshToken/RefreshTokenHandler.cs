using AutoMapper;
using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Streetcode.Auth.Models.DTO;
using Streetcode.Auth.Models.Entities;
using Streetcode.Auth.Services.Interfaces.Logging;
using Streetcode.Auth.Services.Interfaces.Users;
using Streetcode.Auth.Settings;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Streetcode.Auth.MediatR.Users.RefreshToken
{
    public class RefreshTokenHandler : IRequestHandler<RefreshTokenCommand, Result<LoginResultDto>>
    {
        private readonly UserManager<User> _userManager;
        private readonly ILoggerService _logger;
        private readonly ITokenService _tokenService;
        private readonly JwtSettings _jwtSettings;
        private readonly IMapper _mapper;

        public RefreshTokenHandler(
            UserManager<User> userManager,
            ILoggerService logger,
            ITokenService tokenService,
            JwtSettings jwtSettings,
            IMapper mapper)
        {
            _userManager = userManager;
            _logger = logger;
            _tokenService = tokenService;
            _jwtSettings = jwtSettings;
            _mapper = mapper;
        }

        public async Task<Result<LoginResultDto>> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var (jwtToken, newRefreshToken) = await _tokenService.RefreshTokenAsync(request.RefreshTokenRequest.RefreshToken);

                var tokenString = new JwtSecurityTokenHandler().WriteToken(jwtToken);

                var userId = jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

                if (userId == null) return Result.Fail("Could not retrieve user from token.");

                var user = await _userManager.FindByIdAsync(userId);
                if (user == null) return Result.Fail("User not found.");

                _logger.LogInformation($"Token refreshed successfully for user {user.Id}");

                return Result.Ok(new LoginResultDto
                {
                    User = _mapper.Map<UserDto>(user),
                    Token = tokenString,
                    RefreshToken = newRefreshToken,
                    ExpireAt = jwtToken.ValidTo,
                });
            }
            catch (SecurityTokenException ex)
            {
                _logger.LogError(request, $"Refresh failed: {ex.Message}");
                return Result.Fail<LoginResultDto>("Invalid or expired refresh token.");
            }
        }
    }
}