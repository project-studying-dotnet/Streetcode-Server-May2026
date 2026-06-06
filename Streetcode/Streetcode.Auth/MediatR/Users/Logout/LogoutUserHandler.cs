using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Streetcode.Auth.Models.Entities;
using Streetcode.Auth.Services.Interfaces.Logging;
using Streetcode.Auth.Services.Interfaces.Users;
using Streetcode.Auth.Services.Users;
using Streetcode.Auth.Extensions;

namespace Streetcode.Auth.MediatR.Users.Logout
{
    public class LogoutUserHandler : IRequestHandler<LogoutUserCommand, Result<Unit>>
    {
        private readonly ITokenService _tokenService;
        private readonly ILoggerService _logger;
        public LogoutUserHandler(ITokenService tokenService, ILoggerService logger)
        {
            _tokenService = tokenService;
            _logger = logger;
        }

        //public async Task<Result<Unit>> Handle(LogoutUserCommand request, CancellationToken cancellationToken)
        //{
        //    var user = await _userManager.FindByIdAsync(request.UserId.ToString());

        //    if (user is null)
        //    {
        //        string errorMsg = $"User with id '{request.UserId}' not found.";
        //        _logger.LogError(request, errorMsg);
        //        return Result.Fail(errorMsg);
        //    }

        //    user.EnsureSecurityStamp();
        //    user.RefreshToken = null;
        //    user.RefreshTokenExpiryTime = null;

        //    await _userManager.UpdateAsync(user);

        //    _logger.LogInformation($"User {user.Id} logged out");

        //    return Result.Ok(Unit.Value);
        //}

        public async Task<Result<Unit>> Handle(LogoutUserCommand request, CancellationToken cancellationToken)
        {
            try
            {
                await _tokenService.RevokeTokenAsync(request.RefreshToken);
                _logger.LogInformation("User session revoked successfully.");
                return Result.Ok(Unit.Value);
            }
            catch (Exception ex)
            {
                _logger.LogError(request, $"Logout failed: {ex.Message}");
                return Result.Fail("Failed to revoke session.");
            }
        }
    }
}
