using AutoMapper;
using Moq;
using Streetcode.BLL.DTO.Streetcode.TextContent.Term;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.Mapping.Streetcode.TextContent;
using Streetcode.BLL.MediatR.Streetcode.Term.Create;
using Streetcode.DAL.Repositories.Interfaces.Base;
using Xunit;

using TermEntity = Streetcode.DAL.Entities.Streetcode.TextContent.Term;

namespace Streetcode.XUnitTest.BLL.MediatR.StreetCode.Term.Create
{
    public class CreateTermHandlerTests
    {
        private readonly IMapper _mapper;
        private readonly Mock<IRepositoryWrapper> _repositoryWrapperMock;
        private readonly Mock<ILoggerService> _loggerMock;

        private readonly CreateTermHandler _handler;

        public CreateTermHandlerTests()
        {
            _mapper = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<TermProfile>();
            }).CreateMapper();

            _repositoryWrapperMock = new Mock<IRepositoryWrapper>();
            _loggerMock = new Mock<ILoggerService>();

            _handler = new CreateTermHandler(
                _mapper,
                _repositoryWrapperMock.Object,
                _loggerMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldReturnFail_WhenMapperReturnsNull()
        {
            var request = new CreateTermCommand(null!);

            var result = await _handler.Handle(request, CancellationToken.None);

            Assert.True(result.IsFailed);
            Assert.Equal("Cannot create new term!", result.Errors[0].Message);

            _loggerMock.Verify(l => l.LogError(request, "Cannot create new term!"), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldReturnSuccess_WhenTermIsCreated()
        {
            var termDto = new CreateTermDto { Title = "Test Title", Description = "Test Description" };
            var request = new CreateTermCommand(termDto);

            _repositoryWrapperMock.Setup(r => r.TermRepository.GetAllAsync(It.IsAny<System.Linq.Expressions.Expression<Func<TermEntity, bool>>>(), null))
                                  .ReturnsAsync(new List<TermEntity>());

            _repositoryWrapperMock.Setup(r => r.TermRepository.CreateAsync(It.IsAny<TermEntity>()))
                                  .Returns((TermEntity n) => Task.FromResult(n));

            _repositoryWrapperMock.Setup(r => r.SaveChangesAsync())
                                  .ReturnsAsync(1);

            var result = await _handler.Handle(request, CancellationToken.None);

            Assert.True(result.IsSuccess);

            Assert.Equal(termDto.Title, result.Value.Title);
            Assert.Equal(termDto.Description, result.Value.Description);

            _repositoryWrapperMock.Verify(r => r.TermRepository.CreateAsync(It.IsAny<TermEntity>()), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldReturnFail_WhenSaveChangesReturnsZero()
        {
            var request = new CreateTermCommand(new CreateTermDto { Title = "Test Title", Description = "Test Description" });

            TermEntity capturedEntity = null!;

            _repositoryWrapperMock.Setup(r => r.TermRepository.GetAllAsync(It.IsAny<System.Linq.Expressions.Expression<Func<TermEntity, bool>>>(), null))
                                  .ReturnsAsync(new List<TermEntity>());

            _repositoryWrapperMock.Setup(r => r.TermRepository.CreateAsync(It.IsAny<TermEntity>()))
                                  .Callback<TermEntity>(n => capturedEntity = n)
                                  .Returns((TermEntity n) => Task.FromResult(n));

            _repositoryWrapperMock.Setup(r => r.SaveChangesAsync())
                                  .ReturnsAsync(0);

            var result = await _handler.Handle(request, CancellationToken.None);

            Assert.True(result.IsFailed);
            Assert.Equal("Cannot save changes in the database after term creation!", result.Errors[0].Message);

            _loggerMock.Verify(l => l.LogError(request, "Cannot save changes in the database after term creation!"), Times.Once);

            Assert.NotNull(capturedEntity);
        }
    }
}
