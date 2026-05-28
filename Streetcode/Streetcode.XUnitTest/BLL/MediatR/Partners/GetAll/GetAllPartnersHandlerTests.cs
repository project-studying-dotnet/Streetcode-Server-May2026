using System.Linq.Expressions;
using AutoMapper;
using FluentAssertions;
using Microsoft.EntityFrameworkCore.Query;
using Moq;
using Streetcode.BLL.DTO.Partners;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.MediatR.Partners.GetAll;
using Streetcode.DAL.Entities.Partners;
using Streetcode.DAL.Repositories.Interfaces.Base;
using Streetcode.DAL.Repositories.Interfaces.Partners;
using Xunit;
using Streetcode.BLL.Resources;

namespace Streetcode.XUnitTest.BLL.MediatR.Partners.GetAll
{
    public class GetAllPartnersHandlerTests
    {
        private readonly Mock<IRepositoryWrapper> _mockRepo;
        private readonly Mock<IPartnersRepository> _mockPartnersRepo;
        private readonly IMapper _mapper;
        private readonly Mock<ILoggerService> _mockLogger;
        private readonly GetAllPartnersHandler _handler;

        public GetAllPartnersHandlerTests()
        {
            _mockRepo = new Mock<IRepositoryWrapper>();
            _mockPartnersRepo = new Mock<IPartnersRepository>();

            _mapper = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Partner, PartnerDTO>();
            }).CreateMapper();

            _mockLogger = new Mock<ILoggerService>();

            _mockRepo.Setup(x => x.PartnersRepository).Returns(_mockPartnersRepo.Object);

            _handler = new GetAllPartnersHandler(_mockRepo.Object, _mapper, _mockLogger.Object);
        }

        [Fact]
        public async Task Handle_ShouldReturnPartners_WhenPositionsExist()
        {
            var partners = new List<Partner>
            {
                new Partner
                {
                    Id = 1, Title = "Title 1", LogoId = 1,
                    IsKeyPartner = true, IsVisibleEverywhere = true,
                },
                new Partner
                {
                    Id = 2, Title = "Title 2", LogoId = 2,
                    IsKeyPartner = false, IsVisibleEverywhere = false,
                },
            };

            _mockPartnersRepo.Setup(repo => repo.GetAllAsync(
                It.IsAny<Expression<Func<Partner, bool>>>(),
                It.IsAny<Func<IQueryable<Partner>,
                IIncludableQueryable<Partner, object>>>())).ReturnsAsync(partners);

            var result = await _handler.Handle(new GetAllPartnersQuery(), CancellationToken.None);

            result.IsSuccess.Should().BeTrue();
            result.Value.Should().HaveCount(2);
            result.Value.Should().BeEquivalentTo(_mapper.Map<IEnumerable<PartnerDTO>>(partners));
        }

        [Fact]
        public async Task Handle_ShouldReturnFailure_WhenNoPartnersExist()
        {
            _mockPartnersRepo.Setup(repo => repo.GetAllAsync(
                It.IsAny<Expression<Func<Partner, bool>>>(),
                It.IsAny<Func<IQueryable<Partner>,
                IIncludableQueryable<Partner, object>>>())).ReturnsAsync((IEnumerable<Partner>)null!);

            var result = await _handler.Handle(new GetAllPartnersQuery(), CancellationToken.None);

            result.IsFailed.Should().BeTrue();
            result.Errors.Should().ContainSingle(e => e.Message == ErrorMessages.CannotFindAnyPartners);
        }
    }
}
