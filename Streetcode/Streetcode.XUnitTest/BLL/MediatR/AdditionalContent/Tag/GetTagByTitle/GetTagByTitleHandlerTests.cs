using AutoMapper;
using Moq;
using Streetcode.BLL.DTO.AdditionalContent;
using Streetcode.BLL.DTO.AdditionalContent.Tag;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.MediatR.AdditionalContent.Tag.GetByStreetcodeId;
using Streetcode.BLL.MediatR.AdditionalContent.Tag.GetTagByTitle;
using Streetcode.DAL.Repositories.Interfaces.Base;
using System.Linq.Expressions;
using Xunit;

namespace Streetcode.XUnitTest.BLL.MediatR.AdditionalContent.Tag.GetTagByTitle
{
    public class GetTagByTitleHandlerTests
    {
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<IRepositoryWrapper> _repositoryWrapperMock;
        private readonly Mock<ILoggerService> _loggerMock;
        private readonly GetTagByTitleHandler _handler;

        public GetTagByTitleHandlerTests()
        {
            _mapperMock = new Mock<IMapper>();
            _repositoryWrapperMock = new Mock<IRepositoryWrapper>();
            _loggerMock = new Mock<ILoggerService>();
            _handler = new GetTagByTitleHandler(_repositoryWrapperMock.Object, _mapperMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task Handle_TagExists_ReturnsOkResult()
        {
            var query = new GetTagByTitleQuery("TestTitle");
            var tag = new global::Streetcode.DAL.Entities.AdditionalContent.Tag { Title = "TestTitle" };
            var dto = new TagDTO { Title = "TestTitle" };

            _repositoryWrapperMock.Setup(r => r.TagRepository.GetFirstOrDefaultAsync(
                It.IsAny<Expression<Func<global::Streetcode.DAL.Entities.AdditionalContent.Tag, bool>>>(), null))
                .ReturnsAsync(tag);

            _mapperMock.Setup(m => m.Map<TagDTO>(It.IsAny<object>()))
                .Returns(dto);

            var result = await _handler.Handle(query, CancellationToken.None);

            Assert.True(result.IsSuccess);
            Assert.Equal(dto, result.Value);
        }

        [Fact]
        public async Task Handle_TagDoesNotExist_ReturnsFailResult()
        {
            var query = new GetTagByTitleQuery("MissingTitle");

            _repositoryWrapperMock.Setup(r => r.TagRepository.GetFirstOrDefaultAsync(
                It.IsAny<Expression<Func<global::Streetcode.DAL.Entities.AdditionalContent.Tag, bool>>>(), null))
                .ReturnsAsync((global::Streetcode.DAL.Entities.AdditionalContent.Tag)null!);

            var result = await _handler.Handle(query, CancellationToken.None);

            Assert.False(result.IsSuccess);
            Assert.NotEmpty(result.Errors);
        }
    }
}