using AutoMapper;
using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.DAL.Entities.Users;
using Streetcode.DAL.Enums;

namespace Streetcode.BLL.MediatR.Users.Register
{
    public class RegisterUserHandler : IRequestHandler<RegisterUserCommand, Result<Unit>>
    {
        private readonly UserManager<User> _userManager;
        private readonly IMapper _mapper;
        private readonly ILoggerService _logger;

        public RegisterUserHandler(UserManager<User> userManager, IMapper mapper, ILoggerService logger)
        {
            _userManager = userManager;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<Result<Unit>> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Register attempt for {request.registerRequest.Email}");

            if (request.registerRequest.Password != request.registerRequest.PasswordConfirmation)
            {
                string errorMsg = $"User password: {request.registerRequest.Password}, did not match with confirmation password: {request.registerRequest.PasswordConfirmation}.";
                _logger.LogError(request, errorMsg);

                return Result.Fail<Unit>("Passwords do not match");
            }

            var existingUser = await _userManager.FindByEmailAsync(request.registerRequest.Email);

            if (existingUser != null)
            {
                string errorMsg = $"User with email: {request.registerRequest.Email} already exist.";
                _logger.LogError(request, errorMsg);

                return Result.Fail<Unit>("User already exists");
            }

            var user = _mapper.Map<User>(request.registerRequest);

            user.UserName = request.registerRequest.Email;

            var result = await _userManager.CreateAsync(user, request.registerRequest.Password);

            if (!result.Succeeded)
            {
                return Result.Fail<Unit>(result.Errors.Select(e => e.Description));
            }

            await _userManager.AddToRoleAsync(user, UserRole.Moderator.ToString());

            return Result.Ok(Unit.Value);

        }
    }
}
