using System.IdentityModel.Tokens.Jwt;
using AutoMapper;
using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Streetcode.Auth.Models.DTO;
using Streetcode.Auth.Models.Entities;
using Streetcode.Auth.Services.Interfaces.Logging;
using Streetcode.Auth.Extensions;
using Streetcode.Auth.MediatR.Users.Login;
using Streetcode.Auth.Services.Interfaces.Users;

namespace Streetcode.BLL.MediatR.Users.Login
{
    public class LoginUserHandler : IRequestHandler<LoginUserCommand, Result<LoginResultDto>>
    {
        private readonly UserManager<User> _userManager;
        private readonly IMapper _mapper;
        private readonly ILoggerService _logger;
        private readonly ITokenService _tokenService;

        public LoginUserHandler(
            UserManager<User> userManager,
            IMapper mapper,
            ILoggerService logger,
            ITokenService tokenService)
        {
            _userManager = userManager;
            _mapper = mapper;
            _logger = logger;
            _tokenService = tokenService;
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

            var jwtToken = _tokenService.GenerateJWTToken(user);
            var token = new JwtSecurityTokenHandler().WriteToken(jwtToken);
            var refreshToken = _tokenService.GenerateRefreshToken();

            await _tokenService.SaveRefreshTokenAsync(user.Id, refreshToken);

            user.EnsureSecurityStamp();
            await _userManager.UpdateAsync(user);

            _logger.LogInformation($"User {user.Id} successfully logged in");

            var userDto = _mapper.Map<UserDto>(user);

            var loginResult = new LoginResultDto
            {
                User = userDto,
                Token = token,
                RefreshToken = refreshToken,
                ExpireAt = jwtToken.ValidTo,
            };

            return Result.Ok(loginResult);
        }
    }
}