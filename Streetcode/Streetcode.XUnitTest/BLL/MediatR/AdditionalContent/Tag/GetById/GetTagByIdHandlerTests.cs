using AutoMapper;
using Moq;
using Streetcode.BLL.DTO.AdditionalContent;
using Streetcode.BLL.DTO.AdditionalContent.Tag;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.MediatR.AdditionalContent.Tag.GetById;
using Streetcode.DAL.Repositories.Interfaces.Base;
using System;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Streetcode.XUnitTest.BLL.MediatR.AdditionalContent.Tag.GetById
{
    public class GetTagByIdHandlerTests
    {
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<IRepositoryWrapper> _repositoryWrapperMock;
        private readonly Mock<ILoggerService> _loggerMock;
        private readonly GetTagByIdHandler _handler;

        public GetTagByIdHandlerTests()
        {
            _mapperMock = new Mock<IMapper>();
            _repositoryWrapperMock = new Mock<IRepositoryWrapper>();
            _loggerMock = new Mock<ILoggerService>();
            _handler = new GetTagByIdHandler(_repositoryWrapperMock.Object, _mapperMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task Handle_TagExists_ReturnsOkResult()
        {
            var query = new GetTagByIdQuery(1);
            var tag = new Streetcode.DAL.Entities.AdditionalContent.Tag { Id = 1 };
            var dto = new TagDTO { Id = 1 };

            _repositoryWrapperMock.Setup(r => r.TagRepository.GetFirstOrDefaultAsync(
                It.IsAny<Expression<Func<Streetcode.DAL.Entities.AdditionalContent.Tag, bool>>>(), null))
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
            var query = new GetTagByIdQuery(1);

            _repositoryWrapperMock.Setup(r => r.TagRepository.GetFirstOrDefaultAsync(
                It.IsAny<Expression<Func<Streetcode.DAL.Entities.AdditionalContent.Tag, bool>>>(), null))
                .ReturnsAsync((Streetcode.DAL.Entities.AdditionalContent.Tag)null!);

            var result = await _handler.Handle(query, CancellationToken.None);

            Assert.False(result.IsSuccess);
            Assert.NotEmpty(result.Errors);
        }
    }
}