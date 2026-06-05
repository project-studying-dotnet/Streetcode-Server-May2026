using System.Linq.Expressions;
using AutoMapper;
using Moq;
using Streetcode.BLL.DTO.AdditionalContent;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.MediatR.AdditionalContent.Tag.GetAll;
using Streetcode.DAL.Repositories.Interfaces.Base;
using Xunit;
using FluentAssertions;

namespace Streetcode.XUnitTest.BLL.MediatR.AdditionalContent.Tag.GetAll
{
    public class GetAllTagsHandlerTests
    {
        private readonly IMapper _mapper;
        private readonly Mock<IRepositoryWrapper> _repositoryWrapperMock;
        private readonly Mock<ILoggerService> _loggerMock;
        private readonly GetAllTagsHandler _handler;

        public GetAllTagsHandlerTests()
        {
            _mapper = new MapperConfiguration(cfg =>
            {
                cfg.AddMaps(typeof(GetAllTagsHandler).Assembly);
            }).CreateMapper();
            _repositoryWrapperMock = new Mock<IRepositoryWrapper>();
            _loggerMock = new Mock<ILoggerService>();
            _handler = new GetAllTagsHandler(_repositoryWrapperMock.Object, _mapper, _loggerMock.Object);
        }

        [Fact]
        public async Task Handle_TagsExist_ReturnsOkResult()
        {
            var query = new GetAllTagsQuery();
            var tags = new List<global::Streetcode.DAL.Entities.AdditionalContent.Tag>
            {
                new() { Title = "SampleTagTitle" }
            };
            var dtos = _mapper.Map<IEnumerable<TagDTO>>(tags);

            _repositoryWrapperMock.Setup(r => r.TagRepository.GetAllAsync(
                It.IsAny<Expression<Func<global::Streetcode.DAL.Entities.AdditionalContent.Tag, bool>>>(), null))
                .ReturnsAsync(tags);

            var result = await _handler.Handle(query, CancellationToken.None);

            result.IsSuccess.Should().BeTrue();
            result.Value.Should().BeEquivalentTo(dtos);
        }

        [Fact]
        public async Task Handle_TagsNotFound_ReturnsFailResult()
        {
            var query = new GetAllTagsQuery();

            _repositoryWrapperMock.Setup(r => r.TagRepository.GetAllAsync(
                It.IsAny<Expression<Func<global::Streetcode.DAL.Entities.AdditionalContent.Tag, bool>>>(), null))
                .ReturnsAsync((IEnumerable<global::Streetcode.DAL.Entities.AdditionalContent.Tag>)null!);

            var result = await _handler.Handle(query, CancellationToken.None);

            Assert.False(result.IsSuccess);
            Assert.NotEmpty(result.Errors);
        }
    }
}