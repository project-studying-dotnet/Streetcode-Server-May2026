// <copyright file="SendEmailHandlerTests.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
namespace Streetcode.XUnitTest.BLL.MediatR.Email
{
    using System.Threading.Tasks;
    using Moq;
    using Streetcode.BLL.DTO.Email;
    using Streetcode.BLL.Interfaces.Email;
    using Streetcode.BLL.Interfaces.Logging;
    using Streetcode.BLL.MediatR.Email;
    using Streetcode.DAL.Entities.AdditionalContent.Email;
    using Xunit;

    /// <summary>
    /// Unit tests for GetTimelineItemByIdHandler.
    /// </summary>
    public class SendEmailHandlerTests
    {
        private readonly Mock<IEmailService> emailServiceMock;
        private readonly Mock<ILoggerService> loggerMock;

        private readonly SendEmailHandler handler;

        /// <summary>
        /// Initializes a new instance of the <see cref="SendEmailHandlerTests"/> class.
        /// </summary>
        public SendEmailHandlerTests()
        {
            this.emailServiceMock = new Mock<IEmailService>();
            this.loggerMock = new Mock<ILoggerService>();
            this.handler = new SendEmailHandler(
                this.emailServiceMock.Object,
                this.loggerMock.Object);
        }

        /// <summary>
        /// Should return true when item Success.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        [Fact]
        public async Task Handle_Should_Return_Ok_When_Email_Is_Sent()
        {
            var command = CreateCommand();

            this.emailServiceMock
                .Setup(s => s.SendEmailAsync(It.IsAny<Message>()))
                .ReturnsAsync(true);

            var result = await this.handler.Handle(command, CancellationToken.None);

            Assert.True(result.IsSuccess);

            this.emailServiceMock.Verify(
                x => x.SendEmailAsync(It.IsAny<Message>()), Times.Once);

            this.loggerMock.Verify(
                   x => x.LogError(It.IsAny<object>(), It.IsAny<string>()),
                   Times.Never);
        }

        /// <summary>
        /// Should return Fail when email sending fails.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        [Fact]
        public async Task Handle_Should_Return_Fail_When_Email_Sending_Fails()
        {
            var command = CreateCommand();

            this.emailServiceMock
               .Setup(s => s.SendEmailAsync(It.IsAny<Message>()))
               .ReturnsAsync(false);

            var result = await this.handler.Handle(command, CancellationToken.None);

            Assert.True(result.IsFailed);

            this.emailServiceMock.Verify(
                x => x.SendEmailAsync(It.IsAny<Message>()), Times.Once);

            this.loggerMock.Verify(
                   x => x.LogError(It.IsAny<object>(), It.IsAny<string>()),
                   Times.Once);
        }

        private static SendEmailCommand CreateCommand() =>
            new SendEmailCommand(new EmailDTO
            {
                From = "test@test.com",
                Content = "Hello",
            });
    }
}