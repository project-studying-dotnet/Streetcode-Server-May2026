using AutoMapper;
using FluentAssertions;
using Moq;
using Streetcode.BLL.DTO.Streetcode.TextContent.Term;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.Mapping.Streetcode.TextContent;
using Streetcode.BLL.MediatR.Streetcode.Term.Update;
using Streetcode.DAL.Entities.Streetcode.TextContent;
using Streetcode.DAL.Repositories.Interfaces.Base;
using System.Linq.Expressions;
using Xunit;

namespace Streetcode.XUnitTest.BLL.MediatR.StreetCode.Term.Update
{
    public class UpdateTermHandlerTests
    {
        private readonly IMapper _mapper;
        private readonly Mock<IRepositoryWrapper> _repositoryWrapperMock;
        private readonly Mock<ILoggerService> _loggerMock;

        public UpdateTermHandlerTests()
        {
            var mapperConfig = new MapperConfiguration(c =>
            {
                c.AddProfile<TermProfile>();
            });

            _mapper = mapperConfig.CreateMapper();
            _repositoryWrapperMock = new Mock<IRepositoryWrapper>();
            _loggerMock = new Mock<ILoggerService>();
        }

        [Fact]
        public async Task Handle_ReturnsFailResult_WhenTermNotFound()
        {
            var request = new UpdateTermCommand(new UpdateTermDto { Id = 1 });

            _repositoryWrapperMock.Setup(r => r.TermRepository.GetFirstOrDefaultAsync(
                It.IsAny<Expression<Func<DAL.Entities.Streetcode.TextContent.Term, bool>>>(), null))
                .ReturnsAsync((DAL.Entities.Streetcode.TextContent.Term)null);

            var handler = new UpdateTermHandler(_repositoryWrapperMock.Object, _mapper, _loggerMock.Object);

            var result = await handler.Handle(request, CancellationToken.None);

            result.IsFailed.Should().BeTrue();
            result.Errors.Should().Contain(e => e.Message == $"Cannot find a term with corresponding id: {request.Term.Id}");

            _loggerMock.Verify(l => l.LogError(request, $"Cannot find a term with corresponding id: {request.Term.Id}"), Times.Once);

            _repositoryWrapperMock.Verify(r => r.TermRepository.Update(It.IsAny<DAL.Entities.Streetcode.TextContent.Term>()), Times.Never);
        }

        [Fact]
        public async Task Handle_ReturnsFailResult_WhenSaveChangesFails()
        {
            var request = new UpdateTermCommand(new UpdateTermDto { Id = 1, Title = "New Title" });
            var existingTerm = new DAL.Entities.Streetcode.TextContent.Term { Id = 1, Title = "Old Title", Description = "Old Desc" };

            _repositoryWrapperMock.Setup(r => r.TermRepository.GetFirstOrDefaultAsync(
                It.IsAny<Expression<Func<DAL.Entities.Streetcode.TextContent.Term, bool>>>(), null))
                .ReturnsAsync(existingTerm);

            _repositoryWrapperMock.Setup(r => r.SaveChangesAsync()).ReturnsAsync(0);

            var handler = new UpdateTermHandler(_repositoryWrapperMock.Object, _mapper, _loggerMock.Object);

            var result = await handler.Handle(request, CancellationToken.None);

            result.IsFailed.Should().BeTrue();
            result.Errors.Should().Contain(e => e.Message == "Failed to update a term");

            _loggerMock.Verify(l => l.LogError(request, "Failed to update a term"), Times.Once);
        }

        [Fact]
        public async Task Handle_ReturnsOkResult_WhenPartialUpdateIsSuccessful_OnlyTitle()
        {
            var request = new UpdateTermCommand(new UpdateTermDto { Id = 1, Title = "New Title", Description = null });
            var existingTerm = new DAL.Entities.Streetcode.TextContent.Term { Id = 1, Title = "Old Title", Description = "Old Desc" };

            _repositoryWrapperMock.Setup(r => r.TermRepository.GetFirstOrDefaultAsync(
                It.IsAny<Expression<Func<DAL.Entities.Streetcode.TextContent.Term, bool>>>(), null))
                .ReturnsAsync(existingTerm);

            _repositoryWrapperMock.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);

            var handler = new UpdateTermHandler(_repositoryWrapperMock.Object, _mapper, _loggerMock.Object);

            var result = await handler.Handle(request, CancellationToken.None);

            result.IsSuccess.Should().BeTrue();
            result.Value.Title.Should().Be("New Title");
            result.Value.Description.Should().Be("Old Desc");

            _repositoryWrapperMock.Verify(r => r.TermRepository.Update(existingTerm), Times.Once);
            _repositoryWrapperMock.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task Handle_ReturnsOkResult_WhenPartialUpdateIsSuccessful_OnlyDescription()
        {
            var request = new UpdateTermCommand(new UpdateTermDto { Id = 1, Title = "   ", Description = "New Desc" });
            var existingTerm = new DAL.Entities.Streetcode.TextContent.Term { Id = 1, Title = "Old Title", Description = "Old Desc" };

            _repositoryWrapperMock.Setup(r => r.TermRepository.GetFirstOrDefaultAsync(
                It.IsAny<Expression<Func<DAL.Entities.Streetcode.TextContent.Term, bool>>>(), null))
                .ReturnsAsync(existingTerm);

            _repositoryWrapperMock.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);

            var handler = new UpdateTermHandler(_repositoryWrapperMock.Object, _mapper, _loggerMock.Object);

            var result = await handler.Handle(request, CancellationToken.None);

            result.IsSuccess.Should().BeTrue();
            result.Value.Title.Should().Be("Old Title");
            result.Value.Description.Should().Be("New Desc");

            _repositoryWrapperMock.Verify(r => r.TermRepository.Update(existingTerm), Times.Once);
        }

        [Fact]
        public async Task Handle_ReturnsOkResult_WhenFullUpdateIsSuccessful()
        {
            var request = new UpdateTermCommand(new UpdateTermDto { Id = 1, Title = "New Title", Description = "New Desc" });
            var existingTerm = new DAL.Entities.Streetcode.TextContent.Term { Id = 1, Title = "Old Title", Description = "Old Desc" };

            _repositoryWrapperMock.Setup(r => r.TermRepository.GetFirstOrDefaultAsync(
                It.IsAny<Expression<Func<DAL.Entities.Streetcode.TextContent.Term, bool>>>(), null))
                .ReturnsAsync(existingTerm);

            _repositoryWrapperMock.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);

            var handler = new UpdateTermHandler(_repositoryWrapperMock.Object, _mapper, _loggerMock.Object);

            var result = await handler.Handle(request, CancellationToken.None);

            result.IsSuccess.Should().BeTrue();
            result.Value.Title.Should().Be("New Title");
            result.Value.Description.Should().Be("New Desc");

            _repositoryWrapperMock.Verify(r => r.TermRepository.Update(existingTerm), Times.Once);
            _loggerMock.Verify(l => l.LogError(It.IsAny<It.IsAnyType>(), It.IsAny<string>()), Times.Never);
        }
    }
}
