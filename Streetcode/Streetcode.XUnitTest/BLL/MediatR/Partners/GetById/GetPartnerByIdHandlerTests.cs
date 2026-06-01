using System.Linq.Expressions;
using AutoMapper;
using FluentAssertions;
using FluentResults;
using Microsoft.EntityFrameworkCore.Query;
using Moq;
using Streetcode.BLL.DTO.Partners;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.DAL.Entities.Partners;
using Streetcode.DAL.Entities.Streetcode;
using Streetcode.DAL.Repositories.Interfaces.Base;
using Streetcode.DAL.Repositories.Interfaces.Partners;
using Xunit;
using Streetcode.BLL.Resources;

namespace Streetcode.BLL.MediatR.Partners.GetById
{
    public class GetPartnerByIdHandlerTests
    {
        private readonly Mock<IRepositoryWrapper> _repositoryWrapperMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<ILoggerService> _loggerMock;
        private readonly Mock<IPartnersRepository> _partnersRepositoryMock;
        private readonly GetPartnerByIdHandler _handler;

        public GetPartnerByIdHandlerTests()
        {
            _repositoryWrapperMock = new Mock<IRepositoryWrapper>();
            _partnersRepositoryMock = new Mock<IPartnersRepository>();
            _mapperMock = new Mock<IMapper>();
            _loggerMock = new Mock<ILoggerService>();

            _repositoryWrapperMock
                .Setup(wrapper => wrapper.PartnersRepository)
                .Returns(_partnersRepositoryMock.Object);

            _handler = new GetPartnerByIdHandler(
              _repositoryWrapperMock.Object,
              _mapperMock.Object,
              _loggerMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldReturnFail_WhenPartnersIsNull()
        {
            int id = 1;
            var query = new GetPartnerByIdQuery(id);

            _partnersRepositoryMock
                .Setup(repo => repo.GetAllAsync(
                    It.IsAny<Expression<Func<Partner, bool>>>(),
                    It.IsAny<Func<IQueryable<Partner>, IIncludableQueryable<Partner, object>>?>()))
                .ReturnsAsync((IEnumerable<Partner>)null!);

            var result = await _handler.Handle(query, CancellationToken.None);

            result.IsFailed.Should().BeTrue();
            result.Errors[0].Message.Should().Be(string.Format(ErrorMessages.CannotFindPartnerById, id));

            _loggerMock.Verify(
                logger => logger.LogError(query, string.Format(ErrorMessages.CannotFindPartnerById, id)),
                Times.Once);

            _mapperMock.Verify(
                mapper => mapper.Map<IEnumerable<PartnerDTO>>(It.IsAny<IEnumerable<Partner>>()),
                Times.Never);
        }

        [Fact]
        public async Task Handle_ShouldReturnOk_WhenPartnersExist()
        {
            int id = 1;
            var query = new GetPartnerByIdQuery(id);

            var partner = new Partner
            {
                Id = 1,
                Title = "Title 1",
                LogoId = 1, IsKeyPartner = true,
                IsVisibleEverywhere = true,
                Streetcodes = new List<StreetcodeContent>(),
            };
            var partnerDto = new PartnerDTO
            {
                Id = 1,
                Title = "Title 1",
                LogoId = 1,
                IsKeyPartner = true,
                IsVisibleEverywhere = true,
            };

            _partnersRepositoryMock
                .Setup(repo => repo.GetSingleOrDefaultAsync(
                    It.IsAny<Expression<Func<Partner, bool>>>(),
                    It.IsAny<Func<IQueryable<Partner>, IIncludableQueryable<Partner, object>>?>()))
                .Returns(Task.FromResult<Partner?>(partner));

            _mapperMock
                .Setup(mapper => mapper.Map<PartnerDTO>(partner))
                .Returns(partnerDto);

            var result = await _handler.Handle(query, CancellationToken.None);

            result.IsSuccess.Should().BeTrue();
            result.Value.Should().BeEquivalentTo(partnerDto);

            _loggerMock.Verify(
                logger => logger.LogError(It.IsAny<object>(), It.IsAny<string>()),
                Times.Never);
        }
    }
}
