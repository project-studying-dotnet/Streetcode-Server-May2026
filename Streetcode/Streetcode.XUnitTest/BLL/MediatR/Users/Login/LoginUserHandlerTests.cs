// <copyright file="LoginUserHandlerTests.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using AutoMapper;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Moq;
using Streetcode.BLL.DTO.Users;
using Streetcode.BLL.Interfaces.Logging;
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
    private readonly LoginUserHandler _handler;

    public LoginUserHandlerTests()
    {
        var config = new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<User, UserDto>()
                .ForMember(
                    dest => dest.Login,
                    opt => opt.MapFrom(src => src.UserName));
        });

        _mapper = config.CreateMapper();

        _loggerMock = new Mock<ILoggerService>();

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
            _loggerMock.Object);
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

        _userManagerMock
            .Setup(m => m.FindByNameAsync(loginDto.Login))
            .ReturnsAsync(dbUser);

        _userManagerMock
            .Setup(m => m.CheckPasswordAsync(dbUser, loginDto.Password))
            .ReturnsAsync(true);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();

        result.Value.Token.Should().NotBeNullOrEmpty();
        result.Value.ExpireAt.Should().BeAfter(DateTime.UtcNow);

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
    }

    [Fact]
    public async Task Handle_ShouldReturnFail_WhenUserDoesNotExist()
    {
        var loginDto = new UserLoginDto
        {
            Login = "admin",
            Password = "wrongPassword",
        };

        var command = new LoginUserCommand(loginDto);

        _userManagerMock
            .Setup(m => m.FindByNameAsync(loginDto.Login))
            .ReturnsAsync((User?)null);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsFailed.Should().BeTrue();

        _userManagerMock.Verify(
            m => m.FindByNameAsync(loginDto.Login),
            Times.Once);

        _userManagerMock.Verify(
            m => m.CheckPasswordAsync(
                It.IsAny<User>(),
                It.IsAny<string>()),
            Times.Never);

        _loggerMock.Verify(
            l => l.LogError(
                command,
                It.IsAny<string>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldReturnFail_WhenPasswordIsInvalid()
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

        _userManagerMock.Verify(
            m => m.FindByNameAsync(loginDto.Login),
            Times.Once);

        _userManagerMock.Verify(
            m => m.CheckPasswordAsync(dbUser, loginDto.Password),
            Times.Once);

        _loggerMock.Verify(
            l => l.LogError(
                command,
                It.IsAny<string>()),
            Times.Once);
    }
}