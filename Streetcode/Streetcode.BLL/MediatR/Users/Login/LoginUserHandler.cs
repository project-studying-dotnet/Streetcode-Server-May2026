using AutoMapper;
using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Streetcode.BLL.DTO.Users;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.Interfaces.Users;
using Streetcode.DAL.Entities.Users;
using System.IdentityModel.Tokens.Jwt;

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

            if (user is null)
            {
                string errorMsg = $"User with login '{request.loginRequest.Login}' not found.";
                _logger.LogError(request, errorMsg);

                return Result.Fail<LoginResultDto>("Invalid login or password.");
            }

            bool isPasswordValid = await _userManager.CheckPasswordAsync(user, request.loginRequest.Password);

            if (!isPasswordValid)
            {
                string errorMsg = $"Invalid password attempt for login '{request.loginRequest.Login}'.";
                _logger.LogError(request, errorMsg);
                return Result.Fail<LoginResultDto>("Invalid login or password.");
            }

            var jwtToken = _tokenService.GenerateJWTToken(user);
            var token = new JwtSecurityTokenHandler().WriteToken(jwtToken);

            _logger.LogInformation($"User {user.Id} successfully logged in");

            var userDto = _mapper.Map<UserDto>(user);

            var loginResult = new LoginResultDto
            {
                User = userDto,
                Token = token,
                ExpireAt = jwtToken.ValidTo,
            };

            return Result.Ok(loginResult);
        }
    }
}