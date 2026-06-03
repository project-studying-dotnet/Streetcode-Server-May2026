using System.Linq.Expressions;
using AutoMapper;
using Microsoft.EntityFrameworkCore.Query;
using Moq;
using Streetcode.BLL.DTO.AdditionalContent.Subtitles;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.MediatR.AdditionalContent.Subtitle.GetAll;
using Streetcode.DAL.Repositories.Interfaces.Base;
using Xunit;
using FluentAssertions;

namespace Streetcode.XUnitTest.BLL.MediatR.AdditionalContent.Subtitle.GetAll
{
    public class GetAllSubtitlesHandlerTests
    {
        private readonly IMapper _mapper;
        private readonly Mock<IRepositoryWrapper> _repositoryWrapperMock;
        private readonly Mock<ILoggerService> _loggerMock;
        private readonly GetAllSubtitlesHandler _handler;

        public GetAllSubtitlesHandlerTests()
        {
            _mapper = new MapperConfiguration(cfg =>
            {
                cfg.AddMaps(typeof(GetAllSubtitlesHandler).Assembly);
            }).CreateMapper();
            _repositoryWrapperMock = new Mock<IRepositoryWrapper>();
            _loggerMock = new Mock<ILoggerService>();
            _handler = new GetAllSubtitlesHandler(_repositoryWrapperMock.Object, _mapper, _loggerMock.Object);
        }

        [Fact]
        public async Task Handle_SubtitlesExist_ReturnsOkResult()
        {
            var query = new GetAllSubtitlesQuery();
            var subtitles = new List<global::Streetcode.DAL.Entities.AdditionalContent.Subtitle>
            {
                new global::Streetcode.DAL.Entities.AdditionalContent.Subtitle()
            };
            var dtos = _mapper.Map<IEnumerable<SubtitleDTO>>(subtitles);

            _repositoryWrapperMock.Setup(r => r.SubtitleRepository.GetAllAsync(
                It.IsAny<Expression<Func<global::Streetcode.DAL.Entities.AdditionalContent.Subtitle, bool>>>(),
                It.IsAny<Func<IQueryable<global::Streetcode.DAL.Entities.AdditionalContent.Subtitle>, IIncludableQueryable<global::Streetcode.DAL.Entities.AdditionalContent.Subtitle, object>>>()))
                .ReturnsAsync(subtitles);

            var result = await _handler.Handle(query, CancellationToken.None);

            result.IsSuccess.Should().BeTrue();
            result.Value.Should().BeEquivalentTo(dtos);
        }

        [Fact]
        public async Task Handle_SubtitlesNotFound_ReturnsFailResult()
        {
            var query = new GetAllSubtitlesQuery();

            _repositoryWrapperMock.Setup(r => r.SubtitleRepository.GetAllAsync(
                It.IsAny<Expression<Func<global::Streetcode.DAL.Entities.AdditionalContent.Subtitle, bool>>>(),
                It.IsAny<Func<IQueryable<global::Streetcode.DAL.Entities.AdditionalContent.Subtitle>, IIncludableQueryable<global::Streetcode.DAL.Entities.AdditionalContent.Subtitle, object>>>()))
                .ReturnsAsync((IEnumerable<global::Streetcode.DAL.Entities.AdditionalContent.Subtitle>)null!);

            var result = await _handler.Handle(query, CancellationToken.None);

            Assert.False(result.IsSuccess);
        }
    }
}