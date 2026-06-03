using System.Linq.Expressions;
using AutoMapper;
using Moq;
using Streetcode.BLL.DTO.AdditionalContent.Subtitles;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.MediatR.AdditionalContent.GetById;
using Streetcode.BLL.MediatR.AdditionalContent.Subtitle.GetById;
using Streetcode.DAL.Repositories.Interfaces.Base;
using Xunit;

namespace Streetcode.XUnitTest.BLL.MediatR.AdditionalContent.Subtitle.GetById
{
    public class GetSubtitleByIdHandlerTests
    {
        private readonly Mock<IRepositoryWrapper> _mockRepository;
        private readonly Mock<IMapper> _mockMapper;
        private readonly Mock<ILoggerService> _mockLogger;

        public GetSubtitleByIdHandlerTests()
        {
            _mockRepository = new Mock<IRepositoryWrapper>();
            _mockMapper = new Mock<IMapper>();
            _mockLogger = new Mock<ILoggerService>();
        }

        [Fact]
        public async Task Handle_ReturnsOkResult_WhenSubtitleExists()
        {
            int testId = 1;
            var subtitle = new global::Streetcode.DAL.Entities.AdditionalContent.Subtitle { Id = testId };
            var subtitleDTO = new SubtitleDTO { Id = testId };

            _mockRepository.Setup(r => r.SubtitleRepository.GetFirstOrDefaultAsync(
                It.IsAny<Expression<Func<global::Streetcode.DAL.Entities.AdditionalContent.Subtitle, bool>>>(), null))
                .ReturnsAsync(subtitle);

            _mockMapper.Setup(m => m.Map<SubtitleDTO>(It.IsAny<global::Streetcode.DAL.Entities.AdditionalContent.Subtitle>()))
                .Returns(subtitleDTO);

            var handler = new GetSubtitleByIdHandler(_mockRepository.Object, _mockMapper.Object, _mockLogger.Object);
            var result = await handler.Handle(new GetSubtitleByIdQuery(testId), CancellationToken.None);
            Assert.True(result.IsSuccess);
            Assert.Equal(subtitleDTO, result.Value);
        }

        [Fact]
        public async Task Handle_ReturnsFailResult_WhenSubtitleDoesNotExist()
        {
            int testId = 1;
            _mockRepository.Setup(r => r.SubtitleRepository.GetFirstOrDefaultAsync(
                It.IsAny<Expression<Func<global::Streetcode.DAL.Entities.AdditionalContent.Subtitle, bool>>>(), null))
                .ReturnsAsync((global::Streetcode.DAL.Entities.AdditionalContent.Subtitle?)null);
            var handler = new GetSubtitleByIdHandler(_mockRepository.Object, _mockMapper.Object, _mockLogger.Object);

            var result = await handler.Handle(new GetSubtitleByIdQuery(testId), CancellationToken.None);

            Assert.True(result.IsFailed);
            Assert.Equal($"Cannot find a subtitle with corresponding id: {testId}", result.Errors[0].Message);
        }
    }
}