using AutoMapper;
using Moq;
using Streetcode.BLL.DTO.AdditionalContent;
using Streetcode.BLL.DTO.AdditionalContent.Tag;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.MediatR.AdditionalContent.Tag.GetAll;
using Streetcode.DAL.Repositories.Interfaces.Base;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Streetcode.XUnitTest.BLL.MediatR.AdditionalContent.Tag.GetAll
{
    public class GetAllTagsHandlerTests
    {
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<IRepositoryWrapper> _repositoryWrapperMock;
        private readonly Mock<ILoggerService> _loggerMock;
        private readonly GetAllTagsHandler _handler;

        public GetAllTagsHandlerTests()
        {
            _mapperMock = new Mock<IMapper>();
            _repositoryWrapperMock = new Mock<IRepositoryWrapper>();
            _loggerMock = new Mock<ILoggerService>();
            _handler = new GetAllTagsHandler(_repositoryWrapperMock.Object, _mapperMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task Handle_TagsExist_ReturnsOkResult()
        {
            var query = new GetAllTagsQuery();
            var tags = new List<Streetcode.DAL.Entities.AdditionalContent.Tag> { new Streetcode.DAL.Entities.AdditionalContent.Tag() };
            var dtos = new List<TagDTO> { new TagDTO() };

            _repositoryWrapperMock.Setup(r => r.TagRepository.GetAllAsync(
                It.IsAny<Expression<Func<Streetcode.DAL.Entities.AdditionalContent.Tag, bool>>>(), null))
                .ReturnsAsync(tags);

            _mapperMock.Setup(m => m.Map<IEnumerable<TagDTO>>(It.IsAny<IEnumerable<object>>()))
                .Returns(dtos);

            var result = await _handler.Handle(query, CancellationToken.None);

            Assert.True(result.IsSuccess);
            Assert.Equal(dtos, result.Value);
        }

        [Fact]
        public async Task Handle_TagsNotFound_ReturnsFailResult()
        {
            var query = new GetAllTagsQuery();

            _repositoryWrapperMock.Setup(r => r.TagRepository.GetAllAsync(
                It.IsAny<Expression<Func<Streetcode.DAL.Entities.AdditionalContent.Tag, bool>>>(), null))
                .ReturnsAsync((IEnumerable<Streetcode.DAL.Entities.AdditionalContent.Tag>)null!);

            var result = await _handler.Handle(query, CancellationToken.None);

            Assert.False(result.IsSuccess);
            Assert.NotEmpty(result.Errors);
        }
    }
}