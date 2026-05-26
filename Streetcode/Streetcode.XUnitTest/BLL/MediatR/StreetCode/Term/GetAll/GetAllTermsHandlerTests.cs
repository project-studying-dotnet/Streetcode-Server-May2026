using AutoMapper;
using FluentAssertions;
using Moq;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.Mapping.Streetcode.TextContent;
using Streetcode.BLL.MediatR.Streetcode.Term.GetAll;
using Streetcode.DAL.Entities.Streetcode.TextContent;
using Streetcode.DAL.Repositories.Interfaces.Base;
using Xunit;

using Entity = Streetcode.DAL.Entities.Streetcode.TextContent.Term;

namespace Streetcode.XUnitTest.BLL.MediatR.StreetCode.Term.GetAll
{
    public class GetAllTermsHandlerTests
    {
        private readonly IMapper _mapper;
        private readonly Mock<IRepositoryWrapper> _repositoryWrapperMock;
        private readonly Mock<ILoggerService> _loggerMock;

        public GetAllTermsHandlerTests()
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
        public async Task Handle_ShouldReturnOk_WhenTermsExist()
        {
            var terms = new List<Entity>
            {
                new Entity { Id = 1, Title = "Term 1", Description = "Description 1" },
                new Entity { Id = 2, Title = "Term 2", Description = "Description 2" }
            };

            _repositoryWrapperMock.Setup(r => r.TermRepository.GetAllAsync(null, null))
                .ReturnsAsync(terms);

            var handler = new GetAllTermsHandler(_repositoryWrapperMock.Object, _mapper, _loggerMock.Object);

            var result = await handler.Handle(new GetAllTermsQuery(), CancellationToken.None);

            result.IsSuccess.Should().BeTrue();
            result.Value.Should().HaveCount(2);
            result.Value.First().Title.Should().Be("Term 1");
        }

        [Fact]
        public async Task Handle_ShouldReturnFail_WhenTermsAreNull()
        {
            const string expectedErrorMsg = "Cannot find any term";
            _repositoryWrapperMock.Setup(r => r.TermRepository.GetAllAsync(null, null))
                .ReturnsAsync((List<Entity>)null);

            var handler = new GetAllTermsHandler(_repositoryWrapperMock.Object, _mapper, _loggerMock.Object);
            var query = new GetAllTermsQuery();

            var result = await handler.Handle(query, CancellationToken.None);
            result.IsFailed.Should().BeTrue();
            result.Errors.First().Message.Should().Be(expectedErrorMsg);

            _loggerMock.Verify(
                x => x.LogError(query, expectedErrorMsg),
                Times.Once);
        }
    }
}
