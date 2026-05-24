using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore.Query;
using Moq;
using Streetcode.BLL.DTO.AdditionalContent.Subtitles;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.MediatR.AdditionalContent.Subtitle.GetAll;
using Streetcode.DAL.Repositories.Interfaces.Base;
using Xunit;

namespace Streetcode.XUnitTest.BLL.MediatR.AdditionalContent.Subtitle.GetAll
{
    public class GetAllSubtitlesHandlerTests
    {
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<IRepositoryWrapper> _repositoryWrapperMock;
        private readonly Mock<ILoggerService> _loggerMock;
        private readonly GetAllSubtitlesHandler _handler;

        public GetAllSubtitlesHandlerTests()
        {
            _mapperMock = new Mock<IMapper>();
            _repositoryWrapperMock = new Mock<IRepositoryWrapper>();
            _loggerMock = new Mock<ILoggerService>();
            _handler = new GetAllSubtitlesHandler(_repositoryWrapperMock.Object, _mapperMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task Handle_SubtitlesExist_ReturnsOkResult()
        {
            var query = new GetAllSubtitlesQuery();
            var subtitles = new List<global::Streetcode.DAL.Entities.AdditionalContent.Subtitle>
            {
                new global::Streetcode.DAL.Entities.AdditionalContent.Subtitle()
            };
            var dtos = new List<SubtitleDTO> { new SubtitleDTO() };

            _repositoryWrapperMock.Setup(r => r.SubtitleRepository.GetAllAsync(
                It.IsAny<Expression<Func<global::Streetcode.DAL.Entities.AdditionalContent.Subtitle, bool>>>(),
                It.IsAny<Func<IQueryable<global::Streetcode.DAL.Entities.AdditionalContent.Subtitle>, IIncludableQueryable<global::Streetcode.DAL.Entities.AdditionalContent.Subtitle, object>>>()))
                .ReturnsAsync(subtitles);

            _mapperMock.Setup(m => m.Map<IEnumerable<SubtitleDTO>>(It.IsAny<IEnumerable<object>>()))
                .Returns(dtos);

            var result = await _handler.Handle(query, CancellationToken.None);

            Assert.True(result.IsSuccess);
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