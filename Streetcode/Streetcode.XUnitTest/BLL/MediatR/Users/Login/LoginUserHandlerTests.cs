// <copyright file="LoginUserHandlerTests.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using System.Text;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using AutoMapper;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Moq;
using Streetcode.BLL.DTO.Users;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.Interfaces.Users;
using Streetcode.BLL.MediatR.Users.Login;
using Streetcode.DAL.Entities.Users;
using Streetcode.DAL.Enums;
using Xunit;

namespace Streetcode.XUnitTest.BLL.MediatR.Users.Login;

public class LoginUserHandlerTests
{
    private readonly Mock<IUserStore<User>> _userStoreMock;
    private readonly Mock<UserManager<User>> _userManagerMock;
    private readonly IMapper _mapper;
    private readonly Mock<ILoggerService> _loggerMock;
    private readonly Mock<ITokenService> _tokenServiceMock;
    private readonly LoginUserHandler _handler;

    public LoginUserHandlerTests()
    {
        _mapper = new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<User, UserDto>()
                .ForMember(
                    dest => dest.Login,
                    opt => opt.MapFrom(src => src.UserName));
        }).CreateMapper();

        _loggerMock = new Mock<ILoggerService>();
        _tokenServiceMock = new Mock<ITokenService>();
        _userStoreMock = new Mock<IUserStore<User>>();

        _userManagerMock = new Mock<UserManager<User>>(
            _userStoreMock.Object,
            null!,
            null!,
            null!,
            null!,
            null!,
            null!,
            null!,
            null!);

        _handler = new LoginUserHandler(
            _userManagerMock.Object,
            _mapper,
            _loggerMock.Object,
            _tokenServiceMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnLoginResultDto_WhenCredentialsAreValid()
    {
        var loginDto = new UserLoginDto
        {
            Login = "admin",
            Password = "correctPassword",
        };

        var command = new LoginUserCommand(loginDto);

        var dbUser = new User
        {
            Id = 1,
            UserName = loginDto.Login,
            Name = "John",
            Surname = "Doe",
            Email = "john.doe@gmail.com",
            Role = UserRole.MainAdministrator,
        };

        var token = CreateJwtToken(dbUser);

        _userManagerMock
            .Setup(m => m.FindByNameAsync(loginDto.Login))
            .ReturnsAsync(dbUser);

        _userManagerMock
            .Setup(m => m.CheckPasswordAsync(dbUser, loginDto.Password))
            .ReturnsAsync(true);

        _tokenServiceMock
            .Setup(service => service.GenerateJWTToken(dbUser))
            .Returns(token);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();

        result.Value.Token.Should().NotBeNullOrWhiteSpace();
        result.Value.ExpireAt.Should().BeAfter(DateTime.UtcNow);

        var jwtHandler = new JwtSecurityTokenHandler();
        var jwtToken = jwtHandler.ReadJwtToken(result.Value.Token);

        jwtToken.Claims.Should()
            .Contain(claim =>
                claim.Type == ClaimTypes.Role &&
                claim.Value == UserRole.MainAdministrator.ToString());

        result.Value.User.Id.Should().Be(dbUser.Id);
        result.Value.User.Login.Should().Be(loginDto.Login);
        result.Value.User.Name.Should().Be("John");
        result.Value.User.Surname.Should().Be("Doe");
        result.Value.User.Email.Should().Be("john.doe@gmail.com");
        result.Value.User.Role.Should().Be(UserRole.MainAdministrator);

        _userManagerMock.Verify(
            m => m.FindByNameAsync(loginDto.Login),
            Times.Once);

        _userManagerMock.Verify(
            m => m.CheckPasswordAsync(dbUser, loginDto.Password),
            Times.Once);

        _tokenServiceMock.Verify(
            service => service.GenerateJWTToken(dbUser),
            Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldReturnFail_WhenUserDoesNotExist()
    {
        var loginDto = new UserLoginDto
        {
            Login = "wrongUser",
            Password = "anyPassword",
        };

        var command = new LoginUserCommand(loginDto);

        _userManagerMock
            .Setup(m => m.FindByNameAsync(loginDto.Login))
            .ReturnsAsync((User?)null);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsFailed.Should().BeTrue();

        result.Errors.Should()
            .ContainSingle(error =>
                error.Message.Contains("Invalid login or password."));

        _userManagerMock.Verify(
            m => m.FindByNameAsync(loginDto.Login),
            Times.Once);

        _userManagerMock.Verify(
            m => m.CheckPasswordAsync(
                It.IsAny<User>(),
                It.IsAny<string>()),
            Times.Never);

        _tokenServiceMock.Verify(
            service => service.GenerateJWTToken(It.IsAny<User>()),
            Times.Never);

        _loggerMock.Verify(
            logger => logger.LogError(
                command,
                It.IsAny<string>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldReturnFail_WhenPasswordIsIncorrect()
    {
        var loginDto = new UserLoginDto
        {
            Login = "admin",
            Password = "wrongPassword",
        };

        var command = new LoginUserCommand(loginDto);

        var dbUser = new User
        {
            Id = 1,
            UserName = loginDto.Login,
            Name = "John",
            Surname = "Doe",
            Email = "john.doe@gmail.com",
            Role = UserRole.MainAdministrator,
        };

        _userManagerMock
            .Setup(m => m.FindByNameAsync(loginDto.Login))
            .ReturnsAsync(dbUser);

        _userManagerMock
            .Setup(m => m.CheckPasswordAsync(dbUser, loginDto.Password))
            .ReturnsAsync(false);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsFailed.Should().BeTrue();

        result.Errors.Should()
            .ContainSingle(error =>
                error.Message.Contains("Invalid login or password."));

        _userManagerMock.Verify(
            m => m.FindByNameAsync(loginDto.Login),
            Times.Once);

        _userManagerMock.Verify(
            m => m.CheckPasswordAsync(dbUser, loginDto.Password),
            Times.Once);

        _tokenServiceMock.Verify(
            service => service.GenerateJWTToken(It.IsAny<User>()),
            Times.Never);

        _loggerMock.Verify(
            logger => logger.LogError(
                command,
                It.IsAny<string>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldSetSecurityStamp_WhenUserHasNullSecurityStamp()
    {
        var loginDto = new UserLoginDto
        {
            Login = "admin",
            Password = "correctPassword",
        };

        var command = new LoginUserCommand(loginDto);

        var dbUser = new User
        {
            Id = 1,
            UserName = loginDto.Login,
            Name = "John",
            Surname = "Doe",
            Email = "john.doe@gmail.com",
            Role = UserRole.MainAdministrator,
            SecurityStamp = null,
        };

        var token = CreateJwtToken(dbUser);

        _userManagerMock
            .Setup(m => m.FindByNameAsync(loginDto.Login))
            .ReturnsAsync(dbUser);

        _userManagerMock
            .Setup(m => m.CheckPasswordAsync(dbUser, loginDto.Password))
            .ReturnsAsync(true);

        _tokenServiceMock
            .Setup(service => service.GenerateJWTToken(dbUser))
            .Returns(token);

        _userManagerMock
            .Setup(m => m.UpdateAsync(dbUser))
            .ReturnsAsync(IdentityResult.Success);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        dbUser.SecurityStamp.Should().NotBeNullOrWhiteSpace();
        _userManagerMock.Verify(m => m.UpdateAsync(dbUser), Times.Once);
    }

    private static JwtSecurityToken CreateJwtToken(User user)
    {
        const string issuer = "Streetcode.WebApi";
        const string audience = "Streetcode.Client";
        const string key = "StreetcodeSuperSecretJwtKeyForUnitTests1234567890";

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Name, user.UserName ?? string.Empty),
            new(ClaimTypes.Role, user.Role.ToString()),
        };

        var signingKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(key));

        var credentials = new SigningCredentials(
            signingKey,
            SecurityAlgorithms.HmacSha256);

        return new JwtSecurityToken(
            issuer,
            audience,
            claims,
            expires: DateTime.UtcNow.AddHours(2),
            signingCredentials: credentials);
    }
}