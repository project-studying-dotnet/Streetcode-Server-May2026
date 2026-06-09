using AutoMapper;
using FluentAssertions;
using MassTransit;
using Microsoft.AspNetCore.Identity;
using Moq;
using Streetcode.Auth.MediatR.Users.Register;
using Streetcode.Auth.Models.DTO;
using Streetcode.Auth.Models.Entities;
using Streetcode.Auth.Services.Interfaces.Logging;
using Streetcode.Auth.Services.Interfaces.Users;
using Streetcode.Common.Enums;
using Streetcode.Common.Events;
using Xunit;

namespace Streetcode.XUnitTest.AuthService.MediatR.Users.Register;

public class RegisterUserHandlerTests
{
    private readonly Mock<UserManager<User>> _userManagerMock;
    private readonly Mock<ILoggerService> _loggerMock;
    private readonly Mock<IPublishEndpoint> _publishEndpointMock;
    private readonly Mock<IAuthService> _authServiceMock;
    private readonly IMapper _mapper;
    private readonly RegisterUserHandler _handler;

    public RegisterUserHandlerTests()
    {
        _userManagerMock = new Mock<UserManager<User>>(
            Mock.Of<IUserStore<User>>(),
            null!,
            null!,
            null!,
            null!,
            null!,
            null!,
            null!,
            null!);

        _loggerMock = new Mock<ILoggerService>();
        _publishEndpointMock = new Mock<IPublishEndpoint>();
        _authServiceMock = new Mock<IAuthService>();

        var mapperConfig = new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<UserRegisterDto, User>();
            cfg.CreateMap<User, UserDto>();
        });

        _mapper = mapperConfig.CreateMapper();

        _handler = new RegisterUserHandler(
            _userManagerMock.Object,
            _mapper,
            _loggerMock.Object,
            _publishEndpointMock.Object,
            _authServiceMock.Object);
    }

    private static UserRegisterDto CreateValidDto() => new()
    {
        Name = "John",
        Surname = "Doe",
        Email = "john@test.com",
        Password = "Password123!",
        PasswordConfirmation = "Password123!"
    };

    private static RegisterUserCommand CreateCommand(UserRegisterDto dto)
        => new(dto);

    private static User CreateUser(UserRegisterDto dto) => new()
    {
        Id = 1,
        Email = dto.Email,
        Name = dto.Name,
        Surname = dto.Surname
    };

    private static AuthResponseDto CreateAuthResponse(User user) => new()
    {
        User = new UserDto
        {
            Id = user.Id,
            Name = user.Name,
            Surname = user.Surname,
            Email = user.Email!
        },
        Token = "jwt-token",
        RefreshToken = "refresh-token",
        ExpireAt = DateTime.UtcNow.AddHours(1)
    };

    private void SetupUserNotExists(UserRegisterDto dto)
    {
        _userManagerMock
            .Setup(x => x.FindByEmailAsync(dto.Email))
            .ReturnsAsync((User?)null);
    }

    private void SetupUserExists(User user)
    {
        _userManagerMock
            .Setup(x => x.FindByEmailAsync(user.Email!))
            .ReturnsAsync(user);
    }

    [Fact]
    public async Task Handle_ShouldRegisterUser_WhenDataIsValid()
    {
        // Arrange
        var dto = CreateValidDto();
        var command = CreateCommand(dto);
        var user = CreateUser(dto);
        var authResponse = CreateAuthResponse(user);

        SetupUserNotExists(dto);

        _userManagerMock
            .Setup(x => x.CreateAsync(It.IsAny<User>(), dto.Password))
            .ReturnsAsync(IdentityResult.Success);

        _userManagerMock
            .Setup(x => x.AddToRoleAsync(
                It.IsAny<User>(),
                UserRole.Moderator.ToString()))
            .ReturnsAsync(IdentityResult.Success);

        _authServiceMock
            .Setup(x => x.CreateLoginResultAsync(It.IsAny<User>()))
            .ReturnsAsync(authResponse);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();

        result.Value.Should().NotBeNull();
        result.Value.Token.Should().Be("jwt-token");
        result.Value.RefreshToken.Should().Be("refresh-token");

        _authServiceMock.Verify(
            x => x.CreateLoginResultAsync(It.IsAny<User>()),
            Times.Once);

        _userManagerMock.Verify(
            x => x.CreateAsync(It.IsAny<User>(), dto.Password),
            Times.Once);

        _userManagerMock.Verify(
            x => x.AddToRoleAsync(
                It.IsAny<User>(),
                UserRole.Moderator.ToString()),
            Times.Once);

        _publishEndpointMock.Verify(
            x => x.Publish(
                It.IsAny<UserRegisteredEvent>(),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldReturnFail_WhenUserAlreadyExists()
    {
        // Arrange
        var dto = CreateValidDto();
        var user = CreateUser(dto);

        SetupUserExists(user);

        // Act
        var result = await _handler.Handle(
            CreateCommand(dto),
            CancellationToken.None);

        // Assert
        result.IsFailed.Should().BeTrue();

        result.Errors.Should()
            .ContainSingle(e => e.Message == "User already exists");

        _userManagerMock.Verify(
            x => x.CreateAsync(It.IsAny<User>(), It.IsAny<string>()),
            Times.Never);

        _publishEndpointMock.Verify(
            x => x.Publish(
                It.IsAny<UserRegisteredEvent>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldReturnFail_WhenCreateUserFails()
    {
        // Arrange
        var dto = CreateValidDto();

        SetupUserNotExists(dto);

        _userManagerMock
            .Setup(x => x.CreateAsync(It.IsAny<User>(), dto.Password))
            .ReturnsAsync(
                IdentityResult.Failed(
                    new IdentityError
                    {
                        Description = "Password too weak"
                    }));

        // Act
        var result = await _handler.Handle(
            CreateCommand(dto),
            CancellationToken.None);

        // Assert
        result.IsFailed.Should().BeTrue();

        result.Errors.Should()
            .ContainSingle(e => e.Message == "Password too weak");

        _publishEndpointMock.Verify(
            x => x.Publish(
                It.IsAny<UserRegisteredEvent>(),
                It.IsAny<CancellationToken>()),
            Times.Never);

        _authServiceMock.Verify(
            x => x.CreateLoginResultAsync(It.IsAny<User>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldReturnFail_WhenAddToRoleFails()
    {
        // Arrange
        var dto = CreateValidDto();

        SetupUserNotExists(dto);

        _userManagerMock
            .Setup(x => x.CreateAsync(It.IsAny<User>(), dto.Password))
            .ReturnsAsync(IdentityResult.Success);

        _userManagerMock
            .Setup(x => x.AddToRoleAsync(
                It.IsAny<User>(),
                It.IsAny<string>()))
            .ReturnsAsync(
                IdentityResult.Failed(
                    new IdentityError
                    {
                        Description = "Role assignment error"
                    }));

        // Act
        var result = await _handler.Handle(
            CreateCommand(dto),
            CancellationToken.None);

        // Assert
        result.IsFailed.Should().BeTrue();

        result.Errors.Should()
            .ContainSingle(e => e.Message == "Role assignment error");

        _publishEndpointMock.Verify(
            x => x.Publish(
                It.IsAny<UserRegisteredEvent>(),
                It.IsAny<CancellationToken>()),
            Times.Never);

        _authServiceMock.Verify(
            x => x.CreateLoginResultAsync(It.IsAny<User>()),
            Times.Never);
    }
}