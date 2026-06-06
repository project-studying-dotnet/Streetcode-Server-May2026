using System.Linq.Expressions;
using AutoMapper;
using FluentAssertions;
using Microsoft.EntityFrameworkCore.Query;
using Moq;
using Streetcode.BLL.DTO.Partners;
using Streetcode.BLL.DTO.Streetcode;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.MediatR.Partners.Create;
using Streetcode.DAL.Entities.Partners;
using Streetcode.DAL.Entities.Streetcode;
using Streetcode.DAL.Repositories.Interfaces.Base;
using Streetcode.DAL.Repositories.Interfaces.Partners;
using Streetcode.DAL.Repositories.Interfaces.Streetcode;
using Xunit;

namespace Streetcode.XUnitTest.BLL.MediatR.Partners.Create
{
    public class CreatePatnerHandlerTests
    {
        private readonly Mock<IRepositoryWrapper> _repositoryWrapperMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<IStreetcodeRepository> _streetcodeRepoMock;
        private readonly Mock<ILoggerService> _loggerMock;
        private readonly Mock<IPartnersRepository> _partnersRepositoryMock;
        private readonly CreatePartnerHandler _handler;

        public CreatePatnerHandlerTests()
        {
            _repositoryWrapperMock = new Mock<IRepositoryWrapper>();
            _partnersRepositoryMock = new Mock<IPartnersRepository>();
            _streetcodeRepoMock = new Mock<IStreetcodeRepository>();
            _mapperMock = new Mock<IMapper>();
            _loggerMock = new Mock<ILoggerService>();

            _repositoryWrapperMock
                .Setup(r => r.StreetcodeRepository)
                .Returns(_streetcodeRepoMock.Object);

            _repositoryWrapperMock
                .Setup(r => r.PartnersRepository)
                .Returns(_partnersRepositoryMock.Object);

            _streetcodeRepoMock
                .Setup(r => r.GetAllAsync(It.IsAny<Expression<Func<StreetcodeContent, bool>>>()))
                .ReturnsAsync(new List<StreetcodeContent>());

            _handler = new CreatePartnerHandler(
                _repositoryWrapperMock.Object,
                _mapperMock.Object,
                _loggerMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldReturnOk_WhenPartnerCreatedSuccessfully()
        {
            var dto = new PartnerDTO
            {
                Id = 1,
                Title = "Title 1",
                LogoId = 1,
                IsKeyPartner = true,
                IsVisibleEverywhere = true,
            };
            var createdDto = new PartnerDTO
            {
                Id = 1,
                Title = "Title 1",
                LogoId = 1,
                IsKeyPartner = true,
                IsVisibleEverywhere = true,
            };
            var partner = new Partner
            {
                Id = 1,
                Title = "Title 1",
                LogoId = 1,
                IsKeyPartner = true,
                IsVisibleEverywhere = true,
                Streetcodes = new List<StreetcodeContent>(),
            };
            var createPartnerQuery = new CreatePartnerQuery(new CreatePartnerDTO
            {
                Id = 1,
                Title = "Title 1",
                LogoId = 1,
                IsKeyPartner = true,
                IsVisibleEverywhere = true,
                Streetcodes = new List<StreetcodeShortDTO>(),
            });

            _mapperMock
                .Setup(mapper => mapper.Map<Partner>(It.IsAny<CreatePartnerDTO>()))
                .Returns(partner);

            _partnersRepositoryMock
                .Setup(repo => repo.GetAllAsync(
                    It.IsAny<Expression<Func<Partner, bool>>>(),
                    It.IsAny<Func<IQueryable<Partner>, IIncludableQueryable<Partner, object>>?>()))
                .ReturnsAsync(new List<Partner>());

            _partnersRepositoryMock
                .Setup(repo => repo.CreateAsync(It.IsAny<Partner>()))
                .ReturnsAsync(partner);

            _repositoryWrapperMock
                .Setup(wrapper => wrapper.SaveChangesAsync())
                .ReturnsAsync(1);

            _mapperMock
                .Setup(mapper => mapper.Map<PartnerDTO>(It.IsAny<Partner>()))
                .Returns(createdDto);

            var result = await _handler.Handle(createPartnerQuery, CancellationToken.None);

            result.IsSuccess.Should().BeTrue();
            result.Value.Should().BeEquivalentTo(createdDto);

            _partnersRepositoryMock.Verify(
                repo => repo.CreateAsync(It.IsAny<Partner>()),
                Times.Once);

            _repositoryWrapperMock.Verify(
                wrapper => wrapper.SaveChanges(),
                Times.Exactly(2));

            _loggerMock.Verify(
                logger => logger.LogError(It.IsAny<object>(), It.IsAny<string>()),
                Times.Never);
        }

        [Fact]
        public async Task Handle_ShouldReturnFail_WhenRepositoryThrowsException()
        {
            var partner = new Partner
            {
                Id = 1,
                Title = "Title 1",
                Streetcodes = new List<StreetcodeContent>()
            };

            var createPartnerQuery = new CreatePartnerQuery(
                new CreatePartnerDTO
                {
                    Id = 1,
                    Title = "Title 1",
                    LogoId = 1,
                    IsKeyPartner = true,
                    IsVisibleEverywhere = true,
                    Streetcodes = new List<StreetcodeShortDTO>()
                });

            _mapperMock
                .Setup(m => m.Map<Partner>(It.IsAny<CreatePartnerDTO>()))
                .Returns(partner);

            _partnersRepositoryMock
                .Setup(r => r.CreateAsync(It.IsAny<Partner>()))
                .ThrowsAsync(new Exception("Test exception"));

            var result = await _handler.Handle(createPartnerQuery, CancellationToken.None);

            result.IsFailed.Should().BeTrue();
            result.Errors[0].Message.Should().Be("Test exception");

            _loggerMock.Verify(
                logger => logger.LogError(createPartnerQuery, "Test exception"),
                Times.Once);
        }
    }
}
