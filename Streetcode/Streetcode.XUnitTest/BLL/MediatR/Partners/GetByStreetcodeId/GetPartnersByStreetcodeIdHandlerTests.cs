using System.Linq.Expressions;
using AutoMapper;
using FluentAssertions;
using Microsoft.EntityFrameworkCore.Query;
using Moq;
using Streetcode.BLL.DTO.Partners;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.MediatR.Partners.GetByStreetcodeId;
using Streetcode.BLL.Resources;
using Streetcode.DAL.Entities.Partners;
using Streetcode.DAL.Entities.Streetcode;
using Streetcode.DAL.Repositories.Interfaces.Base;
using Streetcode.DAL.Repositories.Interfaces.Partners;
using Streetcode.DAL.Repositories.Interfaces.Streetcode;
using Xunit;

namespace Streetcode.XUnitTest.BLL.MediatR.Partners.GetByStreetcodeId
{
    public class GetPartnersByStreetcodeIdHandlerTests
    {
        private readonly Mock<IRepositoryWrapper> _repositoryWrapperMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<ILoggerService> _loggerMock;
        private readonly Mock<IPartnersRepository> _partnersRepositoryMock;
        private readonly Mock<IStreetcodeRepository> _streetcodeRepositoryMock;
        private readonly GetPartnersByStreetcodeIdHandler _handler;

        public GetPartnersByStreetcodeIdHandlerTests()
        {
            _repositoryWrapperMock = new Mock<IRepositoryWrapper>();
            _partnersRepositoryMock = new Mock<IPartnersRepository>();
            _streetcodeRepositoryMock = new Mock<IStreetcodeRepository>();
            _mapperMock = new Mock<IMapper>();
            _loggerMock = new Mock<ILoggerService>();

            _repositoryWrapperMock
                .Setup(wrapper => wrapper.PartnersRepository)
                .Returns(_partnersRepositoryMock.Object);

            _repositoryWrapperMock
                .Setup(r => r.StreetcodeRepository)
                .Returns(_streetcodeRepositoryMock.Object);

            _handler = new GetPartnersByStreetcodeIdHandler(
              _mapperMock.Object,
              _repositoryWrapperMock.Object,
              _loggerMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldReturnFail_WhenStreetcodeNotFound()
        {
            // Arrange
            int streetcodeId = 1;
            var query = new GetPartnersByStreetcodeIdQuery(streetcodeId);

            _streetcodeRepositoryMock
                .Setup(repo => repo.GetSingleOrDefaultAsync(
                    It.IsAny<Expression<Func<StreetcodeContent, bool>>>(),
                    null))
                .ReturnsAsync((StreetcodeContent)null!);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.IsFailed.Should().BeTrue();
            result.Errors[0].Message.Should().Be(string.Format(ErrorMessages.CannotFindPartnersByStreetcodeId, streetcodeId));

            _loggerMock.Verify(
                logger => logger.LogError(query, string.Format(ErrorMessages.CannotFindPartnersByStreetcodeId, streetcodeId)),
                Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldReturnFail_WhenPartnersNotFound()
        {
            // Arrange
            int streetcodeId = 1;
            var query = new GetPartnersByStreetcodeIdQuery(streetcodeId);
            var streetcode = new StreetcodeContent { Id = streetcodeId };

            _streetcodeRepositoryMock
                .Setup(repo => repo.GetSingleOrDefaultAsync(
                    It.IsAny<Expression<Func<StreetcodeContent, bool>>>(),
                    null))
                .ReturnsAsync(streetcode);

            _partnersRepositoryMock
                .Setup(repo => repo.GetAllAsync(
                    It.IsAny<Expression<Func<Partner, bool>>>(),
                    It.IsAny<Func<IQueryable<Partner>, IIncludableQueryable<Partner, object>>>()))
                .ReturnsAsync((IEnumerable<Partner>)null!);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.IsFailed.Should().BeTrue();
            result.Errors[0].Message.Should().Be(string.Format(ErrorMessages.CannotFindPartnersByStreetcodeId, streetcodeId));

            _loggerMock.Verify(
                logger => logger.LogError(query, string.Format(ErrorMessages.CannotFindPartnersByStreetcodeId, streetcodeId)),
                Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldReturnOk_WhenPartnersExist()
        {
            // Arrange
            int streetcodeId = 1;
            var query = new GetPartnersByStreetcodeIdQuery(streetcodeId);
            var streetcode = new StreetcodeContent { Id = streetcodeId };
            var partners = new List<Partner> { GetDefaultPartnerEntity(streetcode) };
            var partnerDTOs = new List<PartnerDTO> { GetDefaultPartnerDto() };

            _streetcodeRepositoryMock
                .Setup(repo => repo.GetSingleOrDefaultAsync(
                    It.IsAny<Expression<Func<StreetcodeContent, bool>>>(),
                    null))
                .ReturnsAsync(streetcode);

            _partnersRepositoryMock
                .Setup(repo => repo.GetAllAsync(
                    It.IsAny<Expression<Func<Partner, bool>>>(),
                    It.IsAny<Func<IQueryable<Partner>, IIncludableQueryable<Partner, object>>>()))
                .ReturnsAsync(partners);

            _mapperMock
                .Setup(mapper => mapper.Map<IEnumerable<PartnerDTO>>(It.IsAny<IEnumerable<Partner>>()))
                .Returns(partnerDTOs);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().BeEquivalentTo(partnerDTOs);
        }

        #region Test Data Factories

        private static Partner GetDefaultPartnerEntity(StreetcodeContent streetcode) => new()
        {
            Id = 1,
            Title = "Title 1",
            LogoId = 1,
            IsKeyPartner = true,
            IsVisibleEverywhere = true,
            Streetcodes = new List<StreetcodeContent> { streetcode }
        };

        private static PartnerDTO GetDefaultPartnerDto() => new()
        {
            Id = 1,
            Title = "Title 1",
            LogoId = 1,
            IsKeyPartner = true,
            IsVisibleEverywhere = true
        };

        #endregion
    }
}
