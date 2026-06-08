using AutoMapper;
using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Streetcode.Auth.Extensions;
using Streetcode.Auth.MediatR.Users.Login;
using Streetcode.Auth.Models.DTO;
using Streetcode.Auth.Models.Entities;
using Streetcode.Auth.Services.Interfaces.Logging;
using Streetcode.Auth.Services.Interfaces.Users;
using Streetcode.Auth.Services.Users;
using System.IdentityModel.Tokens.Jwt;

namespace Streetcode.BLL.MediatR.Users.Login
{
    public class LoginUserHandler : IRequestHandler<LoginUserCommand, Result<LoginResultDto>>
    {
        private readonly UserManager<User> _userManager;
        private readonly IMapper _mapper;
        private readonly ILoggerService _logger;
        private readonly IAuthService _authService;

        public LoginUserHandler(
            UserManager<User> userManager,
            IMapper mapper,
            ILoggerService logger,
            IAuthService authService)
        {
            _userManager = userManager;
            _mapper = mapper;
            _logger = logger;
            _authService = authService;
        }

        public async Task<Result<LoginResultDto>> Handle(LoginUserCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Login attempt for {request.loginRequest.Login}");

            var user = await _userManager.FindByNameAsync(request.loginRequest.Login);

            if (user is null || !await _userManager.CheckPasswordAsync(user, request.loginRequest.Password))
            {
                _logger.LogError(request, $"Failed login attempt for {request.loginRequest.Login}");
                return Result.Fail<LoginResultDto>("Invalid login or password.");
            }

            var loginResult = await _authService.CreateLoginResultAsync(user);

            _logger.LogInformation($"User {user.Id} successfully logged in");

            return Result.Ok(loginResult);
        }
    }
}