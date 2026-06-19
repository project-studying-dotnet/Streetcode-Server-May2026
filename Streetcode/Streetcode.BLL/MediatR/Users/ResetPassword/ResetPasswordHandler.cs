using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Streetcode.DAL.Entities.Users;

namespace Streetcode.BLL.MediatR.Users.ResetPassword
{
    public class ResetPasswordHandler : IRequestHandler<ResetPasswordCommand, Result<Unit>>
    {
        private readonly UserManager<User> _userManager;

        public ResetPasswordHandler(UserManager<User> userManager)
        {
            _userManager = userManager;
        }

        public async Task<Result<Unit>> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByEmailAsync(request.ResetPasswordDTO.Email);
            if (user == null)
            {
                return Result.Fail("Invalid request or user not found.");
            }

            var result = await _userManager.ResetPasswordAsync(
                user,
                request.ResetPasswordDTO.Token,
                request.ResetPasswordDTO.NewPassword);

            if (result.Succeeded)
            {
                return Result.Ok(Unit.Value);
            }

            return Result.Fail(result.Errors.Select(e => e.Description));
        }
    }
}
