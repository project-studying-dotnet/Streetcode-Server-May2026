using Moq;
using Xunit;
using AutoMapper;
using System.Linq.Expressions;
using Streetcode.BLL.Interfaces.Logging;
using Microsoft.EntityFrameworkCore.Query;
using Streetcode.BLL.DTO.AdditionalContent.Tag;
using Streetcode.DAL.Repositories.Interfaces.Base;
using Streetcode.BLL.MediatR.AdditionalContent.Tag.GetByStreetcodeId;
using AdditionalContentTag = global::Streetcode.DAL.Entities.AdditionalContent.Tag;

namespace Streetcode.XUnitTest.BLL.MediatR.AdditionalContent.Tag.GetByStreetcodeId
{
    public class GetTagByStreetcodeIdHandlerTests
    {
        private readonly IMapper _mapper;
        private readonly Mock<IRepositoryWrapper> _repositoryWrapperMock;
        private readonly Mock<ILoggerService> _loggerMock;
        private readonly GetTagByStreetcodeIdHandler _handler;

        public GetTagByStreetcodeIdHandlerTests()
        {
            _mapper = new MapperConfiguration(cfg =>
            {
                cfg.AddMaps(typeof(GetTagByStreetcodeIdHandler).Assembly);
            }).CreateMapper();
            _repositoryWrapperMock = new Mock<IRepositoryWrapper> { DefaultValue = DefaultValue.Mock };
            _loggerMock = new Mock<ILoggerService>();
            _handler = new GetTagByStreetcodeIdHandler(_repositoryWrapperMock.Object, _mapper, _loggerMock.Object);
        }

        [Fact]
        public async Task Handle_ReturnsOkResult()
        {
            var query = new GetTagByStreetcodeIdQuery(1);
            var list = new List<AdditionalContentTag>
            {
                new()
                {
                    Title = "Test Tag"
                }
            };
            var dtos = _mapper.Map<IEnumerable<StreetcodeTagDTO>>(list);

            _repositoryWrapperMock.Setup(
                repo => repo.TagRepository.GetAllAsync(
                    It.IsAny<Expression<Func<AdditionalContentTag, bool>>>(),
                    It.IsAny<Func<IQueryable<AdditionalContentTag>, IIncludableQueryable<AdditionalContentTag, object>>>()
                )
            ).ReturnsAsync(list);

            var result = await _handler.Handle(query, CancellationToken.None);

            Assert.True(result.IsSuccess);
        }
    }
}