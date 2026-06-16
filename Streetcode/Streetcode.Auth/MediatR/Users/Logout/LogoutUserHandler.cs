using FluentResults;
using MediatR;
using Streetcode.Auth.Services.Interfaces.Logging;
using Streetcode.Auth.Services.Interfaces.Users;

namespace Streetcode.Auth.MediatR.Users.Logout
{
    public class LogoutUserHandler : IRequestHandler<LogoutUserCommand, Result<Unit>>
    {
        private readonly IRefreshTokenService _tokenService;
        private readonly ILoggerService _logger;
        public LogoutUserHandler(IRefreshTokenService tokenService, ILoggerService logger)
        {
            _tokenService = tokenService;
            _logger = logger;
        }
        public async Task<Result<Unit>> Handle(LogoutUserCommand request, CancellationToken cancellationToken)
        {
            try
            {
                await _tokenService.RevokeAsync(request.RefreshToken);
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
