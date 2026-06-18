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

        public GoogleLoginHandler(UserManager<User> userManager, ITokenService tokenService, ILoggerService logger, IMapper mapper)
        {
            _userManager = userManager;
            _tokenService = tokenService;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<Result<LoginResultDto>> Handle(GoogleLoginCommand request, CancellationToken cancellationToken)
        {
            var req = request.googleLoginRequest;
            _logger.LogInformation($"Google login attempt for {req.Email}");

            var user = await _userManager.FindByEmailAsync(req.Email);

            if (user == null)
            {
                user = new User { Email = req.Email, UserName = req.Email, Name = req.Name, Surname = req.Surname };

                var createResult = await _userManager.CreateAsync(user);
                if (IsFailure(createResult, request, $"Failed to create user {req.Email}", out var error))
                {
                    return error;
                }

                var roleResult = await _userManager.AddToRoleAsync(user, UserRole.MainAdministrator.ToString());
                if (IsFailure(roleResult, request, $"Failed to add role for user {user.Id}", out error))
                {
                    return error;
                }
            }

            return Result.Ok(await GenerateLoginResult(user));
        }

        private bool IsFailure(IdentityResult result, object request, string message, out Result<LoginResultDto> failure)
        {
            failure = null!;
            if (result.Succeeded)
            {
                return false;
            }

            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            _logger.LogError(request, $"{message}: {errors}");
            failure = Result.Fail<LoginResultDto>(message);
            return true;
        }
        private async Task<LoginResultDto> GenerateLoginResult(User user)
        {
            var jwtToken = _tokenService.GenerateJWTToken(user);
            var refreshToken = _tokenService.GenerateRefreshToken();

            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
            await _userManager.UpdateAsync(user);

            _logger.LogInformation($"User {user.Id} successfully logged in");

            return new LoginResultDto
            {
                User = _mapper.Map<UserDto>(user),
                Token = new JwtSecurityTokenHandler().WriteToken(jwtToken),
                RefreshToken = refreshToken,
                ExpireAt = jwtToken.ValidTo
            };
        }
    }
}