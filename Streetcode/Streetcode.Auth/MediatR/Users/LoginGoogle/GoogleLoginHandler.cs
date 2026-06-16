using AutoMapper;
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
        private readonly IMapper _mapper;

        private readonly ILoggerService _logger;

        public GoogleLoginHandler(
            UserManager<User> userManager,
            IAuthService authService,
            ILoggerService logger,
            IMapper mapper)
        {
            _userManager = userManager;
            _authService = authService;

            _logger = logger;
            _mapper = mapper;
        }

        public async Task<Result<AuthResponseDto>> Handle(GoogleLoginCommand request, CancellationToken cancellationToken)
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
                    return Result.Fail<AuthResponseDto>("Failed to create user account.");
                }

                var roleResult = await _userManager.AddToRoleAsync(user, UserRole.MainAdministrator.ToString());
                if (!roleResult.Succeeded)
                {
                    _logger.LogError(request, $"Failed to add role for user {user.Id}: {string.Join(", ", roleResult.Errors.Select(e => e.Description))}");
                    return Result.Fail<AuthResponseDto>("User created but failed to assign role.");
                }
            }

            var registrResult = await _authService.CreateLoginResultAsync(user);

            _logger.LogInformation($"User {user.Id} successfully logged in");

            return Result.Ok(registrResult);
        }
    }
}
