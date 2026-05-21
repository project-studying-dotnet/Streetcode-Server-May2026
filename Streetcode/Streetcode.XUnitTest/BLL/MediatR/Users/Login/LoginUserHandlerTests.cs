// <copyright file="LoginUserHandlerTests.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace Streetcode.XUnitTest.BLL.MediatR.Users.Login
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using AutoMapper;
    using FluentAssertions;
    using MockQueryable.Moq;
    using Moq;
    using Streetcode.BLL.DTO.Users;
    using Streetcode.BLL.Interfaces.Logging;
    using Streetcode.BLL.Interfaces.PasswordHasher;
    using Streetcode.BLL.MediatR.Users.Login;
    using Streetcode.DAL.Entities.Users;
    using Streetcode.DAL.Enums;
    using Streetcode.DAL.Repositories.Interfaces.Base;
    using Streetcode.DAL.Repositories.Interfaces.Users;
    using Xunit;

    /// <summary>
    /// Unit tests for LoginUserHandler.
    /// </summary>
    public class LoginUserHandlerTests
    {
        private readonly Mock<IRepositoryWrapper> repoWrapperMock;
        private readonly Mock<IUserRepository> userRepoMock;
        private readonly IMapper mapper;
        private readonly Mock<IPasswordHasher> passwordHasherMock;
        private readonly Mock<ILoggerService> loggerMock;
        private readonly LoginUserHandler handler;

        /// <summary>
        /// Initializes a new instance of the <see cref="LoginUserHandlerTests"/> class.
        /// </summary>
        public LoginUserHandlerTests()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<User, UserDTO>();
            });

            this.mapper = config.CreateMapper();
            this.repoWrapperMock = new Mock<IRepositoryWrapper>();
            this.userRepoMock = new Mock<IUserRepository>();
            this.loggerMock = new Mock<ILoggerService>();
            this.passwordHasherMock = new Mock<IPasswordHasher>();

            this.repoWrapperMock
                .Setup(x => x.UserRepository)
                .Returns(this.userRepoMock.Object);

            this.handler = new LoginUserHandler(
                this.repoWrapperMock.Object,
                this.mapper,
                this.passwordHasherMock.Object,
                this.loggerMock.Object);
        }

        /// <summary>
        /// Should return successful LoginResultDTO with safe UserDTO when credentials are correct.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        [Fact]
        public async Task Handle_ShouldReturnLoginResultDto_WhenCredentialsAreValid()
        {
            var loginDto = new UserLoginDTO { Login = "admin", Password = "correctPassword" };
            var command = new LoginUserCommand(loginDto);

            var dbUser = new User
            {
                Id = 1,
                Login = loginDto.Login,
                PasswordHash = new byte[] { 1, 2, 3 },
                PasswordSalt = new byte[] { 4, 5, 6 },
                Name = "John",
                Surname = "Doe",
                Email = "john.doe@gmail.com",
                Role = UserRole.MainAdministrator,
            };

            var usersMock = new List<User> { dbUser }.AsQueryable().BuildMock();

            this.userRepoMock
                .Setup(r => r.FindAll())
                .Returns(usersMock);

            this.passwordHasherMock
                .Setup(p => p.VerifyPassword(loginDto.Password, dbUser.PasswordHash, dbUser.PasswordSalt))
                .Returns(true);

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

            this.userRepoMock.Verify(r => r.FindAll(), Times.Once);
        }

        /// <summary>
        /// Should return failed Result when requested login does not exist in the database.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        [Fact]
        public async Task Handle_ShouldReturnFail_WhenUserDoesNotExist()
        {
            var loginDto = new UserLoginDTO { Login = "wrongUser", Password = "anyPassword" };
            var command = new LoginUserCommand(loginDto);

            var emptyUsersMock = new List<User>().AsQueryable().BuildMock();
            this.userRepoMock
                .Setup(r => r.FindAll())
                .Returns(emptyUsersMock);

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
            var loginDto = new UserLoginDTO { Login = "admin", Password = "wrongPassword" };
            var command = new LoginUserCommand(loginDto);

            var dbUser = new User
            {
                Login = loginDto.Login,
                Name = "John",
                Surname = "Doe",
                PasswordHash = new byte[] { 1 },
                PasswordSalt = new byte[] { 2 },
            };
            var usersMock = new List<User> { dbUser }.AsQueryable().BuildMock();

            this.userRepoMock
                .Setup(r => r.FindAll())
                .Returns(usersMock);

            this.passwordHasherMock
                .Setup(p => p.VerifyPassword(loginDto.Password, dbUser.PasswordHash, dbUser.PasswordSalt))
                .Returns(false);

            var result = await this.handler.Handle(command, CancellationToken.None);

            result.IsFailed.Should().BeTrue();
            result.Errors.Should().ContainSingle(e => e.Message.Contains("Invalid login or password."));
            this.loggerMock.Verify(l => l.LogError(It.IsAny<object>(), It.IsAny<string>()), Times.Once);
        }
    }
}