using AutoMapper;
using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Streetcode.Auth.Data;
using Streetcode.Auth.Extensions;
using Streetcode.Auth.Models.DTO;
using Streetcode.Auth.Models.Entities;
using Streetcode.Auth.Services;
using Streetcode.Auth.Services.Interfaces;
using Streetcode.Auth.Services.Interfaces.Logging;
using Streetcode.Auth.Services.Interfaces.Users;
using Streetcode.Common.Contracts;
using Streetcode.Common.Enums;

namespace Streetcode.Auth.MediatR.Users.Register
{
    public class RegisterUserHandler : IRequestHandler<RegisterUserCommand, Result<AuthResponseDto>>
    {
        private readonly UserManager<User> _userManager;
        private readonly IMapper _mapper;
        private readonly ILoggerService _logger;
        private readonly IAuthService _authService;
        private readonly ApplicationDbContext _context;
        private readonly IRabbitMqPublisher _rabbitMqPublisher;

        public RegisterUserHandler(
         UserManager<User> userManager,
         IMapper mapper,
         ILoggerService logger,
         IAuthService authService,
         ApplicationDbContext context,
         IRabbitMqPublisher rabbitMqPublisher)
        {
            _userManager = userManager;
            _mapper = mapper;
            _logger = logger;
            _authService = authService;
            _context = context;
            _rabbitMqPublisher = rabbitMqPublisher;
        }

        public async Task<Result<AuthResponseDto>> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Register attempt for {request.registerRequest.Email}");

            var existingUser = await _userManager.FindByEmailAsync(request.registerRequest.Email);
            if (existingUser != null)
            {
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
                return Result.Fail<AuthResponseDto>(roleResult.Errors.Select(e => e.Description));
            }

            await _context.SaveChangesAsync(cancellationToken);

            var registrResult = await _authService.CreateLoginResultAsync(user);

            try
            {
                await _rabbitMqPublisher.PublishAsync(
                     "email-queue",
                     new EmailMessageContract
                     {
                         To = [user.Email!],
                         From = "noreply@streetcode.com",
                         Subject = "Welcome",
                         Content = $"Hello {user.Name}"
                     });
                _logger.LogInformation(">>> [STEP 1] Msg send RabbitMQ!");
            }
            catch (Exception ex)
            {
                _logger.LogError(request, $">>> [STEP 2] Error RabbitMQ: {ex.Message}");
            }

            _logger.LogInformation(">>> [STEP 3] Method End complite RabbitMQ");
            return Result.Ok(registrResult);
        }
    }
}