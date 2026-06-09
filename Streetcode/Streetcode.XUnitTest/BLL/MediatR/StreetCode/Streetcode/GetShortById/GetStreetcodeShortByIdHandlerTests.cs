using AutoMapper;
using Moq;
using Streetcode.BLL.DTO.Streetcode;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.MediatR.Streetcode.Streetcode.GetShortById;
using Streetcode.DAL.Entities.Streetcode;
using Streetcode.DAL.Repositories.Interfaces.Base;
using Xunit;

namespace Streetcode.XUnitTest.BLL.MediatR.StreetCode.Streetcode.GetShortById
{
    public class GetStreetcodeShortByIdHandlerTests
    {
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<IRepositoryWrapper> _repositoryMock;
        private readonly Mock<ILoggerService> _loggerMock;
        private readonly GetStreetcodeShortByIdHandler _handler;

        public GetStreetcodeShortByIdHandlerTests()
        {
            _mapperMock = new Mock<IMapper>();
            _repositoryMock = new Mock<IRepositoryWrapper>();
            _loggerMock = new Mock<ILoggerService>();
            _handler = new GetStreetcodeShortByIdHandler(_mapperMock.Object, _repositoryMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task Should_Return_Ok_When_Streetcode_Exists()
        {
            int id = 1;
            var streetcode = new StreetcodeContent { Id = id };
            var dto = new StreetcodeShortDTO { Id = id };

            _repositoryMock.Setup(r => r.StreetcodeRepository.GetFirstOrDefaultAsync(It.IsAny<System.Linq.Expressions.Expression<Func<StreetcodeContent, bool>>>(), null, default))
                .ReturnsAsync(streetcode);
            _mapperMock.Setup(m => m.Map<StreetcodeShortDTO>(streetcode)).Returns(dto);

            var result = await _handler.Handle(new GetStreetcodeShortByIdQuery(id), default);

            Assert.True(result.IsSuccess);
            Assert.Equal(dto, result.Value);
        }

        [Fact]
        public async Task Should_Return_Fail_When_Streetcode_Is_Null()
        {
            _repositoryMock.Setup(r => r.StreetcodeRepository.GetFirstOrDefaultAsync(It.IsAny<System.Linq.Expressions.Expression<Func<StreetcodeContent, bool>>>(), null, default))
                .ReturnsAsync((StreetcodeContent?)null);

            var result = await _handler.Handle(new GetStreetcodeShortByIdQuery(1), default);

            Assert.True(result.IsFailed);
            Assert.Equal("Cannot find streetcode by id", result.Errors.First().Message);
            _loggerMock.Verify(l => l.LogError(It.IsAny<GetStreetcodeShortByIdQuery>(), It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async Task Should_Return_Fail_When_Mapping_Returns_Null()
        {
            var streetcode = new StreetcodeContent { Id = 1 };
            _repositoryMock.Setup(r => r.StreetcodeRepository.GetFirstOrDefaultAsync(It.IsAny<System.Linq.Expressions.Expression<Func<StreetcodeContent, bool>>>(), null, default))
                .ReturnsAsync(streetcode);

            _mapperMock.Setup(m => m.Map<StreetcodeShortDTO>(streetcode))
                    .Returns((StreetcodeShortDTO)null!);

            var result = await _handler.Handle(new GetStreetcodeShortByIdQuery(1), default);

            Assert.True(result.IsFailed);
            Assert.Equal("Cannot map streetcode to shortDTO", result.Errors.First().Message);
        }
    }
}
