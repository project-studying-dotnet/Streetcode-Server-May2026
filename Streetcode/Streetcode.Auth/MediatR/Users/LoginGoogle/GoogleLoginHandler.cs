using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Streetcode.Auth.Models.DTO;
using Streetcode.Auth.Models.Entities;
using Streetcode.Auth.Services.Interfaces.Logging;
using Streetcode.Auth.Services.Interfaces.Users;
using Streetcode.Common.Enums;

namespace Streetcode.Auth.MediatR.Users.LoginGoogle
{
    public class GoogleLoginHandler : IRequestHandler<GoogleLoginCommand, Result<AuthResponseDto>>
    {
        private readonly UserManager<User> _userManager;
        private readonly IAuthService _authService;

        private readonly ILoggerService _logger;

        public GoogleLoginHandler(
            UserManager<User> userManager,
            IAuthService authService,
            ILoggerService logger)
        {
            _userManager = userManager;
            _authService = authService;

            _logger = logger;
        }

        public async Task<Result<AuthResponseDto>> Handle(GoogleLoginCommand request, CancellationToken cancellationToken)
        {
            var loginDto = request.googleLoginRequest;
            _logger.LogInformation($"Google login attempt for {loginDto.Email}");

            var user = await _userManager.FindByEmailAsync(loginDto.Email);

            if (user == null)
            {
                user = new User { Email = loginDto.Email, UserName = loginDto.Email, Name = loginDto.Name, Surname = loginDto.Surname };

                var createResult = await _userManager.CreateAsync(user);
                var errorResult = CheckIdentityResult(createResult, request, $"Failed to create user {loginDto.Email}");
                if (errorResult != null) return errorResult;

                var roleResult = await _userManager.AddToRoleAsync(user, UserRole.MainAdministrator.ToString());
                errorResult = CheckIdentityResult(roleResult, request, $"Failed to add role for user {user.Id}");
                if (errorResult != null) return errorResult;
            }

            var authResult = await _authService.CreateLoginResultAsync(user);
            _logger.LogInformation($"User {user.Id} successfully logged in");
            return Result.Ok(authResult);
        }

        private Result<AuthResponseDto> CheckIdentityResult(IdentityResult result, object request, string errorMessage)
        {
            if (result.Succeeded) return null!;

            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            _logger.LogError(request, $"{errorMessage}: {errors}");
            return Result.Fail<AuthResponseDto>(errorMessage);
        }
    }
}
