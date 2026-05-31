using System.Linq.Expressions;
using AutoMapper;
using Moq;
using Streetcode.BLL.DTO.AdditionalContent.Subtitles;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.MediatR.AdditionalContent.Subtitle.GetByStreetcodeId;
using Streetcode.DAL.Entities.AdditionalContent;
using Streetcode.DAL.Repositories.Interfaces.Base;
using Xunit;

namespace Streetcode.XUnitTest.BLL.MediatR.AdditionalContent.Subtitle.GetByStreetcodeId
{
    public class GetSubtitlesByStreetcodeIdHandlerTests
    {
        private readonly Mock<IRepositoryWrapper> _mockRepository;
        private readonly Mock<IMapper> _mockMapper;
        private readonly Mock<ILoggerService> _mockLogger;

        public GetSubtitlesByStreetcodeIdHandlerTests()
        {
            _mockRepository = new Mock<IRepositoryWrapper>();
            _mockMapper = new Mock<IMapper>();
            _mockLogger = new Mock<ILoggerService>();
        }

        [Fact]
        public async Task Handle_ReturnsOkResult_WhenSubtitleExistsForStreetcode()
        {
            int testStreetcodeId = 1;
            var subtitle = new global::Streetcode.DAL.Entities.AdditionalContent.Subtitle { StreetcodeId = testStreetcodeId };
            var subtitleDTO = new SubtitleDTO { StreetcodeId = testStreetcodeId };

            _mockRepository.Setup(r => r.SubtitleRepository.GetFirstOrDefaultAsync(
                It.IsAny<Expression<Func<global::Streetcode.DAL.Entities.AdditionalContent.Subtitle, bool>>>(), null))
                .ReturnsAsync(subtitle);

            _mockMapper.Setup(m => m.Map<SubtitleDTO>(It.IsAny<global::Streetcode.DAL.Entities.AdditionalContent.Subtitle>()))
                .Returns(subtitleDTO);

            var handler = new GetSubtitlesByStreetcodeIdHandler(_mockRepository.Object, _mockMapper.Object, _mockLogger.Object);

            var result = await handler.Handle(new GetSubtitlesByStreetcodeIdQuery(testStreetcodeId), CancellationToken.None);

            Assert.True(result.IsSuccess);
            Assert.Equal(subtitleDTO, result.Value);
        }

        [Fact]
        public async Task Handle_ReturnsOkResultWithNull_WhenSubtitleIsNull()
        {
            int testStreetcodeId = 1;

            _mockRepository.Setup(r => r.SubtitleRepository.GetFirstOrDefaultAsync(
                It.IsAny<Expression<Func<global::Streetcode.DAL.Entities.AdditionalContent.Subtitle, bool>>>(), null))
                .ReturnsAsync((global::Streetcode.DAL.Entities.AdditionalContent.Subtitle)null);

            _mockMapper.Setup(m => m.Map<SubtitleDTO>(It.IsAny<global::Streetcode.DAL.Entities.AdditionalContent.Subtitle>()))
                .Returns((SubtitleDTO)null);

            var handler = new GetSubtitlesByStreetcodeIdHandler(_mockRepository.Object, _mockMapper.Object, _mockLogger.Object);

            var result = await handler.Handle(new GetSubtitlesByStreetcodeIdQuery(testStreetcodeId), CancellationToken.None);

            Assert.True(result.IsSuccess);
            Assert.Null(result.Value);
        }
    }
}