using Moq;
using Xunit;
using Microsoft.AspNetCore.Identity;
using Streetcode.BLL.MediatR.Users.ForgotPassword;
using Streetcode.DAL.Entities.Users;
using Streetcode.BLL.Contracts;
using Streetcode.BLL.Interfaces.Email;
using Streetcode.BLL.DTO.Users;

namespace Streetcode.XUnitTest.BLL.MediatR.Users.ForgotPassword
{
    public class ForgotPasswordHandlerTests
    {
        private readonly Mock<UserManager<User>> _userManagerMock;
        private readonly Mock<IEmailPublisher> _rabbitPublisherMock;
        private readonly Mock<ILocalEmailPublisher> _localPublisherMock;
        private readonly ForgotPasswordHandler _handler;

        public ForgotPasswordHandlerTests()
        {
            var store = new Mock<IUserStore<User>>();
            _userManagerMock = new Mock<UserManager<User>>(store.Object, null!, null!, null!, null!, null!, null!, null!, null!);

            _rabbitPublisherMock = new Mock<IEmailPublisher>();
            _localPublisherMock = new Mock<ILocalEmailPublisher>();

            _handler = new ForgotPasswordHandler(
                _userManagerMock.Object,
                _rabbitPublisherMock.Object,
                _localPublisherMock.Object);
        }

        [Fact]
        public async Task Handle_UserNotFound_ReturnsOk()
        {
            // Arrange
            var forgotPasswordDto = new ForgotPasswordDto { Email = "test@test.com" };
            var command = new ForgotPasswordCommand(forgotPasswordDto);

            _userManagerMock.Setup(x => x.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync((User?)null!);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);
            _rabbitPublisherMock.Verify(x => x.PublishAsync(It.IsAny<EmailMessageContract>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Handle_UserFound_PublishesEmail()
        {
            // Arrange
            var user = new User
            {
                Email = "test@test.com",
                Name = "TestName",
                Surname = "TestSurname"
            };

            var forgotPasswordDto = new ForgotPasswordDto { Email = "test@test.com" };
            var command = new ForgotPasswordCommand(forgotPasswordDto);

            _userManagerMock.Setup(x => x.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync(user);
            _userManagerMock.Setup(x => x.GeneratePasswordResetTokenAsync(user)).ReturnsAsync("fake-token");

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);
            _rabbitPublisherMock.Verify(
                x =>
                    x.PublishAsync(
                    It.Is<EmailMessageContract>(m => m.To.Contains("test@test.com")),
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }
    }
}
