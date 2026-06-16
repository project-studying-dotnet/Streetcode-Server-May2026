using AutoMapper;
using FluentAssertions;
using FluentResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Moq;
using Streetcode.Auth.Data;
using Streetcode.Auth.MediatR.Users.Register;
using Streetcode.Auth.Models.DTO;
using Streetcode.Auth.Models.Entities;
using Streetcode.Auth.Services.Interfaces;
using Streetcode.Auth.Services.Interfaces.Logging;
using Streetcode.Auth.Services.Interfaces.Users;
using Streetcode.Common.Contracts;
using Streetcode.Common.Enums;
using Xunit;

namespace Streetcode.XUnitTest.AuthService.MediatR.Users.Register;

public class RegisterUserHandlerTests
{
    private readonly Mock<UserManager<User>> _userManagerMock;
    private readonly Mock<ILoggerService> _loggerMock;
    private readonly Mock<IAuthService> _authServiceMock;
    private readonly Mock<IRabbitMqPublisher> _rabbitMock;
    private readonly Mock<ApplicationDbContext> _contextMock;
    private readonly IMapper _mapper;
    private readonly RegisterUserHandler _handler;

    public RegisterUserHandlerTests()
    {
        _userManagerMock = new Mock<UserManager<User>>(
            Mock.Of<IUserStore<User>>(), null!, null!, null!, null!, null!, null!, null!, null!);

        _loggerMock = new Mock<ILoggerService>();
        _authServiceMock = new Mock<IAuthService>();
        _rabbitMock = new Mock<IRabbitMqPublisher>();

        _contextMock = new Mock<ApplicationDbContext>(
            new DbContextOptions<ApplicationDbContext>());

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
            _authServiceMock.Object,
            _contextMock.Object,
            _rabbitMock.Object);
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

    [Fact]
    public async Task Handle_ShouldRegisterUser_WhenDataIsValid()
    {
        // Arrange
        var dto = CreateValidDto();

        _userManagerMock.Setup(x => x.FindByEmailAsync(dto.Email))
            .ReturnsAsync((User?)null);

        _userManagerMock.Setup(x => x.CreateAsync(It.IsAny<User>(), dto.Password))
            .ReturnsAsync(IdentityResult.Success);

        _userManagerMock.Setup(x => x.AddToRoleAsync(It.IsAny<User>(), UserRole.Moderator.ToString()))
            .ReturnsAsync(IdentityResult.Success);

        _authServiceMock.Setup(x => x.CreateLoginResultAsync(It.IsAny<User>()))
            .ReturnsAsync(new AuthResponseDto
            {
                Token = "jwt-token",
                RefreshToken = "refresh-token",
                User = new UserDto
                {
                    Email = dto.Email,
                    Name = dto.Name,
                    Surname = dto.Surname
                }
            });

        _contextMock.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        _rabbitMock.Setup(x => x.PublishAsync(
                "email-queue",
                It.IsAny<EmailMessageContract>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _handler.Handle(CreateCommand(dto), CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Token.Should().Be("jwt-token");

        _rabbitMock.Verify(
            x => x.PublishAsync(
                "email-queue",
                It.IsAny<EmailMessageContract>()),
            Times.Once);

        _userManagerMock.Verify(
            x => x.CreateAsync(It.IsAny<User>(), dto.Password),
            Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldReturnFail_WhenUserAlreadyExists()
    {
        var dto = CreateValidDto();

        var user = new User
        {
            Email = "test@test.com",
            UserName = "test@test.com",
            Name = "Test",
            Surname = "User",
            Role = UserRole.Moderator
        };

        _userManagerMock.Setup(x => x.FindByEmailAsync(dto.Email))
            .ReturnsAsync(user);

        var result = await _handler.Handle(CreateCommand(dto), CancellationToken.None);

        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainSingle(e => e.Message == "User already exists");

        _rabbitMock.Verify(
            x => x.PublishAsync(It.IsAny<string>(), It.IsAny<EmailMessageContract>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldReturnFail_WhenCreateUserFails()
    {
        var dto = CreateValidDto();

        _userManagerMock.Setup(x => x.FindByEmailAsync(dto.Email))
            .ReturnsAsync((User?)null);

        _userManagerMock.Setup(x => x.CreateAsync(It.IsAny<User>(), dto.Password))
            .ReturnsAsync(IdentityResult.Failed(new IdentityError
            {
                Description = "Password too weak"
            }));

        var result = await _handler.Handle(CreateCommand(dto), CancellationToken.None);

        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainSingle(e => e.Message == "Password too weak");

        _rabbitMock.Verify(
            x => x.PublishAsync(It.IsAny<string>(), It.IsAny<EmailMessageContract>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldReturnFail_WhenAddToRoleFails()
    {
        var dto = CreateValidDto();

        _userManagerMock.Setup(x => x.FindByEmailAsync(dto.Email))
            .ReturnsAsync((User?)null);

        _userManagerMock.Setup(x => x.CreateAsync(It.IsAny<User>(), dto.Password))
            .ReturnsAsync(IdentityResult.Success);

        _userManagerMock.Setup(x => x.AddToRoleAsync(It.IsAny<User>(), It.IsAny<string>()))
            .ReturnsAsync(IdentityResult.Failed(new IdentityError
            {
                Description = "Role assignment error"
            }));

        var result = await _handler.Handle(CreateCommand(dto), CancellationToken.None);

        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainSingle(e => e.Message == "Role assignment error");

        _rabbitMock.Verify(
            x => x.PublishAsync(It.IsAny<string>(), It.IsAny<EmailMessageContract>()),
            Times.Never);
    }
}