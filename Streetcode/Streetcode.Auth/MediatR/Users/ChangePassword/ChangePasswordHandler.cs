using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Streetcode.Auth.Models.Entities;

namespace Streetcode.Auth.Models.MediatR.Users.ChangePassword
{
    public class ChangePasswordHandler : IRequestHandler<ChangePasswordCommand, Result<Unit>>
    {
        private readonly UserManager<User> _userManager;

        public ChangePasswordHandler(UserManager<User> userManager)
        {
            _userManager = userManager;
        }

        public async Task<Result<Unit>> Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
        {
            var requestedUser = await _userManager.FindByIdAsync(request.UserId.ToString());
            if (requestedUser is null)
            {
                return Result.Fail("User not found");
            }

            var passwordUpdateResult = await _userManager.ChangePasswordAsync(
                requestedUser,
                request.ChangePasswordRequest.CurrentPassword,
                request.ChangePasswordRequest.NewPassword);

            if (passwordUpdateResult.Succeeded)
            {
                return Result.Ok(Unit.Value);
            }

            var errorMessages = passwordUpdateResult.Errors.Select(error => error.Description);
            return Result.Fail(errorMessages);
        }
    }
}
