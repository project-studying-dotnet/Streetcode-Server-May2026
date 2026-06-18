using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Streetcode.DAL.Entities.Users;

namespace Streetcode.BLL.MediatR.Users.ChangePassword
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
            var user = await _userManager.FindByIdAsync(request.UserId.ToString());
            if (user == null)
               {
                return Result.Fail("User not found");
            }

            var result = await _userManager.ChangePasswordAsync(
                user,
                request.ChangePasswordRequest.CurrentPassword,
                request.ChangePasswordRequest.NewPassword);

            return result.Succeeded
                ? Result.Ok(Unit.Value)
                : Result.Fail(result.Errors.Select(e => e.Description));
        }
    }
}
