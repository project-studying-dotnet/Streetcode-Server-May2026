using AutoMapper;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Moq;
using Streetcode.BLL.DTO.Users;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.Mapping.Users;
using Streetcode.BLL.MediatR.Users.Register;
using Streetcode.DAL.Entities.Users;
using Streetcode.DAL.Enums;
using Xunit;

namespace Streetcode.XUnitTest.BLL.MediatR.Users.Register;

public class RegisterUserHandlerTests
{
    private readonly Mock<IUserStore<User>> _userStoreMock;
    private readonly Mock<UserManager<User>> _userManagerMock;
    private readonly Mock<ILoggerService> _loggerMock;
    private readonly IMapper _mapper;
    private readonly RegisterUserHandler _handler;

    public RegisterUserHandlerTests()
    {
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

        _loggerMock = new Mock<ILoggerService>();

        var mapperConfig = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<UserProfile>();
        });

        _mapper = mapperConfig.CreateMapper();

        _handler = new RegisterUserHandler(
            _userManagerMock.Object,
            _mapper,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldRegisterUser_WhenDataIsValid()
    {
        // Arrange
        var dto = new UserRegisterDto
        {
            Name = "John",
            Surname = "Doe",
            Email = "john@test.com",
            Password = "Password123!",
            PasswordConfirmation = "Password123!"
        };

        var command = new RegisterUserCommand(dto);

        _userManagerMock
            .Setup(x => x.FindByEmailAsync(dto.Email))
            .ReturnsAsync((User?)null);

        _userManagerMock
            .Setup(x => x.CreateAsync(
                It.IsAny<User>(),
                dto.Password))
            .ReturnsAsync(IdentityResult.Success);

        _userManagerMock
            .Setup(x => x.AddToRoleAsync(
                It.IsAny<User>(),
                UserRole.Moderator.ToString()))
            .ReturnsAsync(IdentityResult.Success);

        // Act
        var result = await _handler.Handle(
            command,
            CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();

        _userManagerMock.Verify(
            x => x.FindByEmailAsync(dto.Email),
            Times.Once);

        _userManagerMock.Verify(
            x => x.CreateAsync(
                It.Is<User>(u =>
                    u.Email == dto.Email &&
                    u.Name == dto.Name &&
                    u.Surname == dto.Surname),
                dto.Password),
            Times.Once);

        _userManagerMock.Verify(
            x => x.AddToRoleAsync(
                It.IsAny<User>(),
                UserRole.Moderator.ToString()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldReturnFail_WhenUserAlreadyExists()
    {
        // Arrange
        var dto = new UserRegisterDto
        {
            Name = "John",
            Surname = "Doe",
            Email = "john@test.com",
            Password = "Password123!",
            PasswordConfirmation = "Password123!"
        };

        var command = new RegisterUserCommand(dto);

        var existingUser = new User
        {
            Name = "Existing",
            Surname = "User",
            Email = dto.Email
        };

        _userManagerMock
            .Setup(x => x.FindByEmailAsync(dto.Email))
            .ReturnsAsync(existingUser);

        // Act
        var result = await _handler.Handle(
            command,
            CancellationToken.None);

        // Assert
        result.IsFailed.Should().BeTrue();

        result.Errors.Should()
            .ContainSingle(x =>
                x.Message.Contains("User already exists"));

        _userManagerMock.Verify(
            x => x.CreateAsync(
                It.IsAny<User>(),
                It.IsAny<string>()),
            Times.Never);

        _userManagerMock.Verify(
            x => x.AddToRoleAsync(
                It.IsAny<User>(),
                It.IsAny<string>()),
            Times.Never);

        _loggerMock.Verify(
            x => x.LogError(
                command,
                It.IsAny<string>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldReturnFail_WhenCreateUserFails()
    {
        // Arrange
        var dto = new UserRegisterDto
        {
            Name = "John",
            Surname = "Doe",
            Email = "john@test.com",
            Password = "Password123!",
            PasswordConfirmation = "Password123!"
        };

        var command = new RegisterUserCommand(dto);

        _userManagerMock
            .Setup(x => x.FindByEmailAsync(dto.Email))
            .ReturnsAsync((User?)null);

        _userManagerMock
            .Setup(x => x.CreateAsync(
                It.IsAny<User>(),
                dto.Password))
            .ReturnsAsync(
                IdentityResult.Failed(
                    new IdentityError
                    {
                        Description = "Create failed"
                    }));

        // Act
        var result = await _handler.Handle(
            command,
            CancellationToken.None);

        // Assert
        result.IsFailed.Should().BeTrue();

        result.Errors.Should()
            .Contain(x =>
                x.Message.Contains("Create failed"));

        _userManagerMock.Verify(
            x => x.AddToRoleAsync(
                It.IsAny<User>(),
                It.IsAny<string>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldReturnFail_WhenAddToRoleFails()
    {
        // Arrange
        var dto = new UserRegisterDto
        {
            Name = "John",
            Surname = "Doe",
            Email = "john@test.com",
            Password = "Password123!",
            PasswordConfirmation = "Password123!"
        };

        var command = new RegisterUserCommand(dto);

        _userManagerMock
            .Setup(x => x.FindByEmailAsync(dto.Email))
            .ReturnsAsync((User?)null);

        _userManagerMock
            .Setup(x => x.CreateAsync(
                It.IsAny<User>(),
                dto.Password))
            .ReturnsAsync(IdentityResult.Success);

        _userManagerMock
            .Setup(x => x.AddToRoleAsync(
                It.IsAny<User>(),
                UserRole.Moderator.ToString()))
            .ReturnsAsync(
                IdentityResult.Failed(
                    new IdentityError
                    {
                        Description = "Role assignment failed"
                    }));

        // Act
        var result = await _handler.Handle(
            command,
            CancellationToken.None);

        // Assert
        result.IsFailed.Should().BeTrue();

        result.Errors.Should()
            .Contain(x =>
                x.Message.Contains("Role assignment failed"));
    }
}
