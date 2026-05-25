using Moq;
using Xunit;
using FluentAssertions;
using Streetcode.BLL.MediatR.Streetcode.Term.Delete;
using Streetcode.DAL.Repositories.Interfaces.Base;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.DAL.Entities.Streetcode.TextContent;
using System.Linq.Expressions;

using Entity = Streetcode.DAL.Entities.Streetcode.TextContent.Term;

namespace Streetcode.XUnitTest.BLL.MediatR.StreetCode.Term.Delete
{
    public class DeleteTermHandlerTests
    {
        private readonly Mock<IRepositoryWrapper> _repositoryWrapperMock;
        private readonly Mock<ILoggerService> _loggerMock;

        public DeleteTermHandlerTests()
        {
            _repositoryWrapperMock = new Mock<IRepositoryWrapper>();
            _loggerMock = new Mock<ILoggerService>();
        }

        [Fact]
        public async Task Handle_ShouldReturnOk_WhenTermIsDeletedSuccessfully()
        {
            var testId = 1;
            var term = new Entity { Id = testId };
            var command = new DeleteTermCommand(testId);

            _repositoryWrapperMock.Setup(r => r.TermRepository
                .GetFirstOrDefaultAsync(It.IsAny<Expression<Func<Entity, bool>>>(), null))
                .ReturnsAsync(term);

            _repositoryWrapperMock.Setup(r => r.SaveChangesAsync())
                .ReturnsAsync(1);

            var handler = new DeleteTermHandler(_repositoryWrapperMock.Object, _loggerMock.Object);

            var result = await handler.Handle(command, CancellationToken.None);

            result.IsSuccess.Should().BeTrue();
            _repositoryWrapperMock.Verify(r => r.TermRepository.Delete(term), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldReturnFail_WhenTermDoesNotExist()
        {
            var testId = 1;
            var command = new DeleteTermCommand(testId);
            string expectedError = $"No term found by entered Id - {testId}";

            _repositoryWrapperMock.Setup(r => r.TermRepository
                .GetFirstOrDefaultAsync(It.IsAny<Expression<Func<Entity, bool>>>(), null))
                .ReturnsAsync((Entity)null!);

            var handler = new DeleteTermHandler(_repositoryWrapperMock.Object, _loggerMock.Object);

            var result = await handler.Handle(command, CancellationToken.None);

            result.IsFailed.Should().BeTrue();
            result.Errors.First().Message.Should().Be(expectedError);
            _loggerMock.Verify(x => x.LogError(command, expectedError), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldReturnFail_WhenSaveChangesReturnsZero()
        {
            var testId = 1;
            var term = new Entity { Id = testId };
            var command = new DeleteTermCommand(testId);
            string expectedError = "Failed to delete term";

            _repositoryWrapperMock.Setup(r => r.TermRepository
                .GetFirstOrDefaultAsync(It.IsAny<Expression<Func<Entity, bool>>>(), null))
                .ReturnsAsync(term);

            _repositoryWrapperMock.Setup(r => r.SaveChangesAsync())
                .ReturnsAsync(0);

            var handler = new DeleteTermHandler(_repositoryWrapperMock.Object, _loggerMock.Object);

            var result = await handler.Handle(command, CancellationToken.None);

            result.IsFailed.Should().BeTrue();
            result.Errors.First().Message.Should().Be(expectedError);
            _loggerMock.Verify(x => x.LogError(command, expectedError), Times.Once);
        }
    }
}
