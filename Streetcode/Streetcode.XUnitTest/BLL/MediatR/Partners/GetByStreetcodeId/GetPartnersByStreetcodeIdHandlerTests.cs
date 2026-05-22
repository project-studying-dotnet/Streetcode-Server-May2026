using AutoMapper;
using FluentAssertions;
using Moq;
using Streetcode.BLL.DTO.Partners;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.MediatR.Partners.GetByStreetcodeId;
using Streetcode.DAL.Entities.Partners;
using Streetcode.DAL.Repositories.Interfaces.Base;
using Streetcode.DAL.Repositories.Interfaces.Partners;
using Streetcode.DAL.Specifications.Base;
using Xunit;

namespace Streetcode.XUnitTest.BLL.MediatR.Partners.GetByStreetcodeId
{
    public class GetPartnersByStreetcodeIdHandlerTests
    {
        private readonly Mock<IRepositoryWrapper> _repositoryWrapperMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<ILoggerService> _loggerMock;
        private readonly Mock<IPartnersRepository> _partnersRepositoryMock;
        private readonly GetPartnersByStreetcodeIdHandler _handler;

        public GetPartnersByStreetcodeIdHandlerTests()
        {
            _repositoryWrapperMock = new Mock<IRepositoryWrapper>();
            _partnersRepositoryMock = new Mock<IPartnersRepository>();
            _mapperMock = new Mock<IMapper>();
            _loggerMock = new Mock<ILoggerService>();

            _repositoryWrapperMock
                .Setup(wrapper => wrapper.PartnersRepository)
                .Returns(_partnersRepositoryMock.Object);

            _handler = new GetPartnersByStreetcodeIdHandler(
              _mapperMock.Object,
              _repositoryWrapperMock.Object,
              _loggerMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldReturnFail_WhenPartnersNotFound()
        {
            int streetcodeId = 1;
            var query = new GetPartnersByStreetcodeIdQuery(streetcodeId);

            _partnersRepositoryMock
                .Setup(repo => repo.GetAllAsync(It.IsAny<ISpecification<Partner>>()))
                .ReturnsAsync((IEnumerable<Partner>)null!);

            var result = await _handler.Handle(query, CancellationToken.None);

            result.IsFailed.Should().BeTrue();
            result.Errors[0].Message.Should().Be("Cannot find a partners by a streetcode id: " + streetcodeId);

            _loggerMock.Verify(
                logger => logger.LogError(query, "Cannot find a partners by a streetcode id: " + streetcodeId),
                Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldReturnOk_WhenPartnersExist()
        {
            int streetcodeId = 1;
            var query = new GetPartnersByStreetcodeIdQuery(streetcodeId);
            var partners = new List<Partner>
            {
                new Partner {
                    Id = 1,
                    Title = "Title 1",
                    LogoId = 1,
                    IsKeyPartner = true,
                    IsVisibleEverywhere = true,
                },
            };
            var partnerDTOs = new List<PartnerDTO>
            {
                new PartnerDTO { Id = 1,
                    Title = "Title 1",
                    LogoId = 1,
                    IsKeyPartner = true,
                    IsVisibleEverywhere = true },
            };

            _partnersRepositoryMock
                .Setup(repo => repo.GetAllAsync(It.IsAny<ISpecification<Partner>>()))
                .ReturnsAsync(partners);

            _mapperMock
                .Setup(mapper => mapper.Map<IEnumerable<PartnerDTO>>(It.IsAny<IEnumerable<Partner>>()))
                .Returns(partnerDTOs);

            var result = await _handler.Handle(query, CancellationToken.None);

            result.IsSuccess.Should().BeTrue();
            result.Value.Should().BeEquivalentTo(partnerDTOs);
        }
    }
}