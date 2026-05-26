using AutoMapper;
using FluentAssertions;
using Moq;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.Mapping.Streetcode.TextContent;
using Streetcode.BLL.MediatR.Streetcode.Term.GetById;
using Streetcode.DAL.Entities.Streetcode.TextContent;
using Streetcode.DAL.Repositories.Interfaces.Base;
using System.Linq.Expressions;
using Xunit;

using Entity = Streetcode.DAL.Entities.Streetcode.TextContent.Term;

namespace Streetcode.XUnitTest.BLL.MediatR.StreetCode.Term.GetById
{
    public class GetTermByIdHandlerTests
    {
        private readonly IMapper _mapper;
        private readonly Mock<IRepositoryWrapper> _repositoryWrapperMock;
        private readonly Mock<ILoggerService> _loggerMock;

        public GetTermByIdHandlerTests()
        {
            _repositoryWrapperMock = new Mock<IRepositoryWrapper>();
            _loggerMock = new Mock<ILoggerService>();

            var mapperConfig = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<TermProfile>();
            });
            _mapper = mapperConfig.CreateMapper();
        }

        [Fact]
        public async Task Handle_ShouldReturnOk_WhenTermExists()
        {
            int testId = 1;
            var term = new Entity { Id = testId, Title = "Streetcode", Description = "History platform" };

            _repositoryWrapperMock.Setup(r => r.TermRepository.GetFirstOrDefaultAsync(
                It.IsAny<Expression<Func<Entity, bool>>>(),
                null))
                .ReturnsAsync(term);

            var handler = new GetTermByIdHandler(_repositoryWrapperMock.Object, _mapper, _loggerMock.Object);
            var query = new GetTermByIdQuery(testId);

            var result = await handler.Handle(query, CancellationToken.None);

            result.IsSuccess.Should().BeTrue();
            result.Value.Id.Should().Be(testId);
            result.Value.Title.Should().Be("Streetcode");
        }

        [Fact]
        public async Task Handle_ShouldReturnFail_WhenTermDoesNotExist()
        {
            int testId = 999;
            string expectedErrorMsg = $"Cannot find any term with corresponding id: {testId}";

            _repositoryWrapperMock.Setup(r => r.TermRepository.GetFirstOrDefaultAsync(
                It.IsAny<Expression<Func<Entity, bool>>>(),
                null))
                .ReturnsAsync((Entity)null);

            var handler = new GetTermByIdHandler(_repositoryWrapperMock.Object, _mapper, _loggerMock.Object);
            var query = new GetTermByIdQuery(testId);

            var result = await handler.Handle(query, CancellationToken.None);

            result.IsFailed.Should().BeTrue();
            result.Errors.First().Message.Should().Be(expectedErrorMsg);

            _loggerMock.Verify(
                x => x.LogError(query, expectedErrorMsg),
                Times.Once);
        }
    }
}
