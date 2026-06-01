using System.Linq.Expressions;
using AutoMapper;
using FluentAssertions;
using Microsoft.EntityFrameworkCore.Query;
using MockQueryable.Moq;
using Moq;
using Streetcode.BLL.DTO.Streetcode.TextContent;
using Streetcode.BLL.DTO.Streetcode.TextContent.RelatedTerm;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.MediatR.Streetcode.RelatedTerm.Create;
using Streetcode.BLL.Resources;
using Streetcode.DAL.Repositories.Interfaces.Base;
using Streetcode.DAL.Repositories.Interfaces.Streetcode.TextContent;
using Xunit;
using Entity = Streetcode.DAL.Entities.Streetcode.TextContent.RelatedTerm;

namespace Streetcode.XUnitTest.MediatRTests.Streetcode.RelatedTerm.Create
{
    public class CreateRelatedTermHandlerTests
    {
        private readonly Mock<IRepositoryWrapper> _repositoryWrapperMock;
        private readonly Mock<ILoggerService> _loggerMock;
        private readonly Mock<IRelatedTermRepository> _relatedTermRepositoryMock;
        private readonly IMapper _mapper;
        private readonly CreateRelatedTermHandler _handler;

        public CreateRelatedTermHandlerTests()
        {
            _repositoryWrapperMock = new Mock<IRepositoryWrapper>();
            _relatedTermRepositoryMock = new Mock<IRelatedTermRepository>();
            _loggerMock = new Mock<ILoggerService>();

            MapperConfiguration config = new(cfg =>
            {
                cfg.AddMaps(typeof(CreateRelatedTermHandler).Assembly);
            });
            _mapper = config.CreateMapper();

            _repositoryWrapperMock.Setup(x => x.RelatedTermRepository).Returns(_relatedTermRepositoryMock.Object);

            _handler = new CreateRelatedTermHandler(
                _repositoryWrapperMock.Object,
                _mapper,
                _loggerMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldReturnFail_WhenRelatedTermAlreadyExists()
        {
            var dto = new CreateRelatedTermDTO { TermId = 1, Word = "test" };
            var command = new CreateRelatedTermCommand(dto);

            var existingData = new List<Entity> { new Entity { TermId = 1, Word = "test" } };

            _relatedTermRepositoryMock
                .Setup(x => x.GetAllAsync(
                    It.IsAny<Expression<Func<Entity, bool>>>(),
                    It.IsAny<Func<IQueryable<Entity>, IIncludableQueryable<Entity, object>>>()))
                .ReturnsAsync(existingData);

            var result = await _handler.Handle(command, CancellationToken.None);

            result.IsFailed.Should().BeTrue();
            result.Errors[0].Message.Should().Be(ErrorMessages.RelatedWordAlreadyExists);
        }

        [Fact]
        public async Task Handle_ShouldReturnOk_WhenRelatedTermCreatedSuccessfully()
        {
            var dto = new CreateRelatedTermDTO { TermId = 1, Word = "test" };
            var command = new CreateRelatedTermCommand(dto);

            var emptyData = new List<Entity>().AsQueryable().BuildMock();
            _relatedTermRepositoryMock.Setup(x => x.FindAll()).Returns(emptyData);

            _relatedTermRepositoryMock.Setup(x => x.Create(It.IsAny<Entity>()))
                .Returns(new Entity { Id = 1, TermId = 1, Word = "test" });

            _repositoryWrapperMock.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            var result = await _handler.Handle(command, CancellationToken.None);

            result.IsSuccess.Should().BeTrue();
            result.Value.TermId.Should().Be(1);
        }

        [Fact]
        public async Task Handle_ShouldReturnFail_WhenMapperReturnsNull()
        {
            var command = new CreateRelatedTermCommand(new CreateRelatedTermDTO
            {
                Word = "test",
                TermId = 1
            });

            var mapperMock = new Mock<IMapper>();
            mapperMock.Setup(m => m.Map<Entity>(It.IsAny<CreateRelatedTermDTO>())).Returns((Entity)null!);

            var handler = new CreateRelatedTermHandler(_repositoryWrapperMock.Object, mapperMock.Object, _loggerMock.Object);

            var result = await handler.Handle(command, CancellationToken.None);

            result.IsFailed.Should().BeTrue();
            result.Errors[0].Message.Should().Be(ErrorMessages.CannotCreateRelatedWordForTerm);
            _loggerMock.Verify(x => x.LogError(command, It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldReturnFail_WhenSaveChangesAsyncReturnsZero()
        {
            var dto = new CreateRelatedTermDTO { TermId = 1, Word = "test" };
            var command = new CreateRelatedTermCommand(dto);

            _relatedTermRepositoryMock.Setup(x => x.GetAllAsync(It.IsAny<Expression<Func<Entity, bool>>>(), null))
                .ReturnsAsync(new List<Entity>());

            _repositoryWrapperMock.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(0);

            var result = await _handler.Handle(command, CancellationToken.None);

            result.IsFailed.Should().BeTrue();
            result.Errors[0].Message.Should().Be(ErrorMessages.CannotSaveRelatedWordChanges);
            _loggerMock.Verify(x => x.LogError(command, It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldReturnFail_WhenCreatedRelatedTermDtoIsNull()
        {
            var dto = new CreateRelatedTermDTO { TermId = 1, Word = "test" };
            var command = new CreateRelatedTermCommand(dto);

            _relatedTermRepositoryMock.Setup(x => x.GetAllAsync(It.IsAny<Expression<Func<Entity, bool>>>(), null))
                .ReturnsAsync(new List<Entity>());

            _repositoryWrapperMock.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

            var mapperMock = new Mock<IMapper>();
            mapperMock.Setup(m => m.Map<Entity>(It.IsAny<CreateRelatedTermDTO>())).Returns(new Entity());
            mapperMock.Setup(m => m.Map<RelatedTermDTO>(It.IsAny<Entity>())).Returns((RelatedTermDTO)null!);

            var handler = new CreateRelatedTermHandler(_repositoryWrapperMock.Object, mapperMock.Object, _loggerMock.Object);

            var result = await handler.Handle(command, CancellationToken.None);

            result.IsFailed.Should().BeTrue();
            result.Errors[0].Message.Should().Be(ErrorMessages.CannotMapEntity);
            _loggerMock.Verify(x => x.LogError(command, It.IsAny<string>()), Times.Once);
        }
    }
}