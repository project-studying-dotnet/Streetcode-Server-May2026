using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Streetcode.BLL.Extensions;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.DAL.Entities.Users;

namespace Streetcode.BLL.MediatR.Users.Logout
{
    public class LogoutUserHandler : IRequestHandler<LogoutUserCommand, Result<Unit>>
    {
        private readonly UserManager<User> _userManager;
        private readonly ILoggerService _logger;

        public LogoutUserHandler(UserManager<User> userManager, ILoggerService logger)
        {
            _userManager = userManager;
            _logger = logger;
        }

        public async Task<Result<Unit>> Handle(LogoutUserCommand request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByIdAsync(request.UserId.ToString());

            if (user is null)
            {
                string errorMsg = $"User with id '{request.UserId}' not found.";
                _logger.LogError(request, errorMsg);
                return Result.Fail(errorMsg);
            }

            user.EnsureSecurityStamp();
            user.RefreshToken = null;
            user.RefreshTokenExpiryTime = null;

            await _userManager.UpdateAsync(user);

            _logger.LogInformation($"User {user.Id} logged out");

            return Result.Ok(Unit.Value);
        }
    }
}
