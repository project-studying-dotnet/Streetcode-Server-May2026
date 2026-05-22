// <copyright file="LoginUserHandlerTests.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace Streetcode.XUnitTest.BLL.MediatR.Users.Login
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
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

    /// <summary>
    /// Unit tests for LoginUserHandler.
    /// </summary>
    public class LoginUserHandlerTests
    {
        private readonly Mock<IUserStore<User>> userStoreMock;
        private readonly Mock<UserManager<User>> userManagerMock;
        private readonly IMapper mapper;
        private readonly Mock<ILoggerService> loggerMock;
        private readonly LoginUserHandler handler;

        /// <summary>
        /// Initializes a new instance of the <see cref="LoginUserHandlerTests"/> class.
        /// </summary>
        public LoginUserHandlerTests()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<User, UserDto>()
                    .ForMember(dest => dest.Login, opt => opt.MapFrom(src => src.UserName));
            });

            this.mapper = config.CreateMapper();
            this.loggerMock = new Mock<ILoggerService>();

            this.userStoreMock = new Mock<IUserStore<User>>();
            this.userManagerMock = new Mock<UserManager<User>>(
                 this.userStoreMock.Object,
                 null!,
                 null!,
                 null!,
                 null!,
                 null!,
                 null!,
                 null!,
                 null!);

            this.handler = new LoginUserHandler(
                this.userManagerMock.Object,
                this.mapper,
                this.loggerMock.Object);
        }

        /// <summary>
        /// Should return successful LoginResultDTO with safe UserDTO when credentials are correct.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        [Fact]
        public async Task Handle_ShouldReturnLoginResultDto_WhenCredentialsAreValid()
        {
            var loginDto = new UserLoginDto { Login = "admin", Password = "correctPassword" };
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

            this.userManagerMock
                .Setup(m => m.FindByNameAsync(loginDto.Login))
                .ReturnsAsync(dbUser);

            this.userManagerMock
                .Setup(m => m.CheckPasswordAsync(dbUser, loginDto.Password))
                .ReturnsAsync(true);

            var result = await this.handler.Handle(command, CancellationToken.None);

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

            this.userManagerMock.Verify(m => m.FindByNameAsync(loginDto.Login), Times.Once);
            this.userManagerMock.Verify(m => m.CheckPasswordAsync(dbUser, loginDto.Password), Times.Once);
        }

        /// <summary>
        /// Should return failed Result when requested login does not exist in the database.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        [Fact]
        public async Task Handle_ShouldReturnFail_WhenUserDoesNotExist()
        {
            var loginDto = new UserLoginDto { Login = "wrongUser", Password = "anyPassword" };
            var command = new LoginUserCommand(loginDto);

            this.userManagerMock
                .Setup(m => m.FindByNameAsync(loginDto.Login))
                .ReturnsAsync((User?)null);

            var result = await this.handler.Handle(command, CancellationToken.None);

            result.IsFailed.Should().BeTrue();
            result.Errors.Should().ContainSingle(e => e.Message.Contains("Invalid login or password."));
            this.loggerMock.Verify(l => l.LogError(It.IsAny<object>(), It.IsAny<string>()), Times.Once);
        }

        /// <summary>
        /// Should return failed Result when provided password does not match database record.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        [Fact]
        public async Task Handle_ShouldReturnFail_WhenPasswordIsIncorrect()
        {
            var loginDto = new UserLoginDto { Login = "admin", Password = "wrongPassword" };
            var command = new LoginUserCommand(loginDto);

            var dbUser = new User
            {
                UserName = loginDto.Login,
                Name = "John",
                Surname = "Doe",
            };

            this.userManagerMock
                .Setup(m => m.FindByNameAsync(loginDto.Login))
                .ReturnsAsync(dbUser);

            this.userManagerMock
                .Setup(m => m.CheckPasswordAsync(dbUser, loginDto.Password))
                .ReturnsAsync(false);

            var result = await this.handler.Handle(command, CancellationToken.None);

            result.IsFailed.Should().BeTrue();
            result.Errors.Should().ContainSingle(e => e.Message.Contains("Invalid login or password."));
            this.loggerMock.Verify(l => l.LogError(It.IsAny<object>(), It.IsAny<string>()), Times.Once);
        }
    }
}