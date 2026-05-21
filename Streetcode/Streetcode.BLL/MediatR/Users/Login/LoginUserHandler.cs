using AutoMapper;
using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Streetcode.BLL.DTO.Users;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.Interfaces.PasswordHasher;
using Streetcode.BLL.Interfaces.Users;
using Streetcode.DAL.Repositories.Interfaces.Base;

namespace Streetcode.BLL.MediatR.Users.Login
{
    public class LoginUserHandler : IRequestHandler<LoginUserCommand, Result<LoginResultDTO>>
    {
        private readonly IRepositoryWrapper _repositoryWrapper;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IMapper _mapper;
        private readonly ILoggerService _logger;

        public LoginUserHandler(IRepositoryWrapper repositoryWrapper, IMapper mapper, IPasswordHasher passwordHasher, ILoggerService logger)
        {
            _repositoryWrapper = repositoryWrapper;
            _mapper = mapper;
            _passwordHasher = passwordHasher;
            _logger = logger;
        }

        public async Task<Result<LoginResultDTO>> Handle(LoginUserCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Login attempt for {Login}");

            var user = await _repositoryWrapper.UserRepository
                .FindAll()
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Login == request.LoginDto.Login, cancellationToken);

            if (user is null)
            {
                string errorMsg = $"User with login '{request.LoginDto.Login}' not found.";
                _logger.LogError(request, errorMsg);
                return Result.Fail<LoginResultDTO>("Invalid login or password.");
            }

            bool isPasswordValid = _passwordHasher.VerifyPassword(request.LoginDto.Password, user.PasswordHash, user.PasswordSalt);

            if (!isPasswordValid)
            {
                string errorMsg = $"Invalid password attempt for login '{request.LoginDto.Login}'.";
                _logger.LogError(request, errorMsg);
                return Result.Fail<LoginResultDTO>("Invalid login or password.");
            }

            var token = "generated-jwt-token-here";

            _logger.LogInformation($"User {user.Id} successfully logged in");

            var userDto = _mapper.Map<UserDTO>(user);

            var loginResult = new LoginResultDTO
            {
                User = userDto,
                Token = token,
                ExpireAt = DateTime.UtcNow.AddHours(2)
            };

            return Result.Ok(loginResult);
        }
    }
}