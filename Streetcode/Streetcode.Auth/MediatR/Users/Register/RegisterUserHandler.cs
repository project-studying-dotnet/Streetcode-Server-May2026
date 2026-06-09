using AutoMapper;
using FluentResults;
using MassTransit;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Streetcode.Auth.Extensions;
using Streetcode.Auth.Models.DTO;
using Streetcode.Auth.Models.Entities;
using Streetcode.Auth.Services.Interfaces.Logging;
using Streetcode.Auth.Services.Interfaces.Users;
using Streetcode.Common.Enums;
using Streetcode.Common.Events;

namespace Streetcode.Auth.MediatR.Users.Register
{
    public class RegisterUserHandler : IRequestHandler<RegisterUserCommand, Result<AuthResponseDto>>
    {
        private readonly UserManager<User> _userManager;
        private readonly IMapper _mapper;
        private readonly ILoggerService _logger;
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly IAuthService _authService;

        public RegisterUserHandler(
         UserManager<User> userManager,
         IMapper mapper,
         ILoggerService logger,
         IPublishEndpoint publishEndpoint,
         IAuthService authService)
        {
            _userManager = userManager;
            _mapper = mapper;
            _logger = logger;
            _publishEndpoint = publishEndpoint;
            _authService = authService;
        }

        public async Task<Result<AuthResponseDto>> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Register attempt for {request.registerRequest.Email}");

            var existingUser = await _userManager.FindByEmailAsync(request.registerRequest.Email);

            if (existingUser != null)
            {
                string errorMsg = $"User with email: {request.registerRequest.Email} already exist.";
                _logger.LogError(request, errorMsg);

                return Result.Fail<AuthResponseDto>("User already exists");
            }

            var user = _mapper.Map<User>(request.registerRequest);

            user.UserName = request.registerRequest.Email;
            user.EnsureSecurityStamp();

            var result = await _userManager.CreateAsync(user, request.registerRequest.Password);

            if (!result.Succeeded)
            {
                return Result.Fail<AuthResponseDto>(result.Errors.Select(e => e.Description));
            }

            var roleResult = await _userManager.AddToRoleAsync(user, UserRole.Moderator.ToString());

            if (!roleResult.Succeeded)
            {
                return Result.Fail<AuthResponseDto>(
                    roleResult.Errors.Select(e => e.Description));
            }
          
            var registrResult = await _authService.CreateLoginResultAsync(user);

            _logger.LogInformation($"User {user.Id} registered successfully.");

            var userDto = _mapper.Map<UserDto>(user);

            try
            {
                await _publishEndpoint.Publish(new UserRegisteredEvent(
                  user.Id,
                  user.Email!,
                  user.UserName,
                  user.Name,
                  user.Surname
              ), cancellationToken);
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("Publishing message timed out. RabbitMQ might be down.");
            }

            return Result.Ok(registrResult);
        }
    }
}
