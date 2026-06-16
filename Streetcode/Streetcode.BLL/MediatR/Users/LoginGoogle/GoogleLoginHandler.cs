using System.IdentityModel.Tokens.Jwt;
using AutoMapper;
using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Streetcode.BLL.DTO.Users;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.Interfaces.Users;
using Streetcode.DAL.Entities.Users;
using Streetcode.DAL.Enums;

namespace Streetcode.BLL.MediatR.Users.LoginGoogle
{
    public class GoogleLoginHandler : IRequestHandler<GoogleLoginCommand, Result<LoginResultDto>>
    {
        private readonly UserManager<User> _userManager;
        private readonly ITokenService _tokenService;
        private readonly IMapper _mapper;

        private readonly ILoggerService _logger;

        public GoogleLoginHandler(
            UserManager<User> userManager,
            ITokenService tokenService,
            ILoggerService logger,
            IMapper mapper)
        {
            _userManager = userManager;
            _tokenService = tokenService;

            _logger = logger;
            _mapper = mapper;
        }

        public async Task<Result<LoginResultDto>> Handle(GoogleLoginCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Google login attempt for {request.googleLoginRequest.Email}");

            var user = await _userManager.FindByEmailAsync(request.googleLoginRequest.Email);

            if (user == null)
            {
                user = new User
                {
                    Email = request.googleLoginRequest.Email,
                    UserName = request.googleLoginRequest.Email,
                    Name = request.googleLoginRequest.Name,
                    Surname = request.googleLoginRequest.Surname
                };

                var result = await _userManager.CreateAsync(user);
                if (!result.Succeeded)
                {
                    _logger.LogError(request, $"Failed to create user {request.googleLoginRequest.Email}: {string.Join(", ", result.Errors.Select(e => e.Description))}");
                    return Result.Fail<LoginResultDto>("Failed to create user account.");
                }

                var roleResult = await _userManager.AddToRoleAsync(user, UserRole.Client.ToString());
                if (!roleResult.Succeeded)
                {
                    _logger.LogError(request, $"Failed to add role for user {user.Id}: {string.Join(", ", roleResult.Errors.Select(e => e.Description))}");
                    return Result.Fail<LoginResultDto>("User created but failed to assign role.");
                }
            }

            var jwtToken = _tokenService.GenerateJWTToken(user);
            var refreshToken = _tokenService.GenerateRefreshToken();

            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
            await _userManager.UpdateAsync(user);

            _logger.LogInformation($"User {user.Id} successfully logged in");

            return Result.Ok(new LoginResultDto
            {
                User = _mapper.Map<UserDto>(user),
                Token = new JwtSecurityTokenHandler().WriteToken(jwtToken),
                RefreshToken = refreshToken,
                ExpireAt = jwtToken.ValidTo
            });
        }
    }
}
