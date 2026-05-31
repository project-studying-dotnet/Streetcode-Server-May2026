using System.Linq.Expressions;
using AutoMapper;
using FluentAssertions;
using FluentResults;
using Microsoft.EntityFrameworkCore.Query;
using Moq;
using Streetcode.BLL.DTO.Partners;
using Streetcode.BLL.DTO.Streetcode;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.MediatR.Partners.GetById;
using Streetcode.BLL.MediatR.Partners.Update;
using Streetcode.DAL.Entities.Partners;
using Streetcode.DAL.Entities.Streetcode;
using Streetcode.DAL.Repositories.Interfaces.Base;
using Streetcode.DAL.Repositories.Interfaces.Partners;
using Xunit;

namespace Streetcode.XUnitTest.BLL.MediatR.Partners.Update
{
    public class UpdatePartnerHandlerTests
    {
        private readonly Mock<IRepositoryWrapper> _repositoryWrapperMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<ILoggerService> _loggerMock;
        private readonly Mock<IPartnersRepository> _partnersRepositoryMock;
        private readonly Mock<IPartnerSourceLinkRepository> _partnerSourceLinkRepositoryMock;
        private readonly Mock<IPartnerStreetcodeRepository> _partnerStreetcodeRepositoryMock;
        private readonly UpdatePartnerHandler _handler;

        public UpdatePartnerHandlerTests()
        {
            _repositoryWrapperMock = new Mock<IRepositoryWrapper>();
            _partnersRepositoryMock = new Mock<IPartnersRepository>();
            _partnerSourceLinkRepositoryMock = new Mock<IPartnerSourceLinkRepository>();
            _partnerStreetcodeRepositoryMock = new Mock<IPartnerStreetcodeRepository>();
            _mapperMock = new Mock<IMapper>();
            _loggerMock = new Mock<ILoggerService>();

            _repositoryWrapperMock
                .Setup(wrapper => wrapper.PartnersRepository)
                .Returns(_partnersRepositoryMock.Object);

            _repositoryWrapperMock
                .Setup(wrapper => wrapper.PartnerSourceLinkRepository)
                .Returns(_partnerSourceLinkRepositoryMock.Object);

            _repositoryWrapperMock
                .Setup(wrapper => wrapper.PartnerStreetcodeRepository)
                .Returns(_partnerStreetcodeRepositoryMock.Object);

            _handler = new UpdatePartnerHandler(
              _repositoryWrapperMock.Object,
              _mapperMock.Object,
              _loggerMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldReturnOk_WhenPartnerUpdatedSuccessfully()
        {
            var partner = new Partner
            {
                Id = 1,
                Title = "Title 1",
                LogoId = 1,
                IsKeyPartner = true,
                IsVisibleEverywhere = true,
                Streetcodes = new List<StreetcodeContent>(),
                PartnerSourceLinks = new List<PartnerSourceLink>()
            };

            var partnerDto = new PartnerDTO
            {
                Id = 1,
                Title = "Title 1",
                LogoId = 1,
                IsKeyPartner = true,
                IsVisibleEverywhere = true,
                Streetcodes = new List<StreetcodeShortDTO>()
            };

            var query = new UpdatePartnerQuery(
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

            _partnerSourceLinkRepositoryMock
                .Setup(r => r.GetAllAsync(
                    It.IsAny<Expression<Func<PartnerSourceLink, bool>>>(),
                    It.IsAny<Func<IQueryable<PartnerSourceLink>,
                    IIncludableQueryable<PartnerSourceLink, object>>?>()))
                .ReturnsAsync(new List<PartnerSourceLink>());

            _partnerStreetcodeRepositoryMock
                .Setup(r => r.GetAllAsync(
                    It.IsAny<Expression<Func<StreetcodePartner, bool>>>(),
                    It.IsAny<Func<IQueryable<StreetcodePartner>,
                    IIncludableQueryable<StreetcodePartner, object>>?>()))
                .ReturnsAsync(new List<StreetcodePartner>());

            _mapperMock
                .Setup(m => m.Map<PartnerDTO>(It.IsAny<Partner>()))
                .Returns(partnerDto);

            var result = await _handler.Handle(query, CancellationToken.None);

            result.IsSuccess.Should().BeTrue();
            result.Value.Should().BeEquivalentTo(partnerDto);

            _partnersRepositoryMock.Verify(
                r => r.Update(It.IsAny<Partner>()),
                Times.Once);

            _repositoryWrapperMock.Verify(
                r => r.SaveChanges(),
                Times.Exactly(2));
        }

        [Fact]
        public async Task Handle_ShouldReturnFail_WhenExceptionThrown()
        {
            var partner = new Partner
            {
                Id = 1,
                Title = "Title 1",
                Streetcodes = new List<StreetcodeContent>(),
                PartnerSourceLinks = new List<PartnerSourceLink>()
            };

            var query = new UpdatePartnerQuery(
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

            _partnerSourceLinkRepositoryMock
                .Setup(r => r.GetAllAsync(
                    It.IsAny<Expression<Func<PartnerSourceLink, bool>>>(),
                    It.IsAny<Func<IQueryable<PartnerSourceLink>,
                    IIncludableQueryable<PartnerSourceLink, object>>?>()))
                .ThrowsAsync(new Exception("Test exception"));

            var result = await _handler.Handle(query, CancellationToken.None);

            result.IsFailed.Should().BeTrue();
            result.Errors[0].Message.Should().Be("Test exception");

            _loggerMock.Verify(
                l => l.LogError(query, "Test exception"),
                Times.Once);
        }
    }
}
