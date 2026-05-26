using AutoMapper;
using FluentResults;
using Moq;
using FluentAssertions;
using Streetcode.DAL.Entities.Partners;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.MediatR.Partners.Delete;
using Streetcode.DAL.Repositories.Interfaces.Base;
using Streetcode.DAL.Repositories.Interfaces.Partners;
using Streetcode.BLL.DTO.Partners;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Query;
using Xunit;
using Streetcode.BLL.Resources;

namespace Streetcode.XUnitTest.BLL.MediatR.Partners.Delete
{
    public class DeletePartnersHandlerTests
    {
        private readonly Mock<IRepositoryWrapper> _repositoryWrapperMock;
        private readonly Mock<IPartnersRepository> _PartnersRepositoryMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<ILoggerService> _loggerMock;
        private readonly DeletePartnerHandler _handler;

        public DeletePartnersHandlerTests()
        {
            _repositoryWrapperMock = new Mock<IRepositoryWrapper>();
            _mapperMock = new Mock<IMapper>();
            _loggerMock = new Mock<ILoggerService>();
            _PartnersRepositoryMock = new Mock<IPartnersRepository>();

            _repositoryWrapperMock
                .Setup(wrapper => wrapper.PartnersRepository)
                .Returns(_PartnersRepositoryMock.Object);
            _handler = new DeletePartnerHandler(
                    _repositoryWrapperMock.Object,
                    _mapperMock.Object,
                    _loggerMock.Object);

        }

        [Fact]
        public async Task Handle_ShouldReturnFail_PartnerNotFound()
        {
            int id = 1;
            var deletePartnerQuery = new DeletePartnerQuery(id);

            _PartnersRepositoryMock
                .Setup(repo => repo.GetFirstOrDefaultAsync(
                    It.IsAny<Expression<Func<Partner, bool>>>(),
                    null))
                .ReturnsAsync((Partner)null!);

            var result = await _handler.Handle(deletePartnerQuery, CancellationToken.None);

            result.IsFailed.Should().BeTrue();
            result.Errors[0].Message.Should().Be(ErrorMessages.NoPartnerWithSuchId);

            _loggerMock.Verify(
                logger => logger.LogError(deletePartnerQuery, ErrorMessages.NoPartnerWithSuchId),
                Times.Once);

            _PartnersRepositoryMock.Verify(
                repo => repo.Delete(It.IsAny<Partner>()),
                Times.Never);

            _repositoryWrapperMock.Verify(
                wrapper => wrapper.SaveChangesAsync(),
                Times.Never);

            _mapperMock.Verify(
                mapper => mapper.Map<PartnerDTO>(It.IsAny<Partner>()),
                Times.Never);
        }

        [Fact]
        public async Task Handle_ShouldReturnOk_WhenPartnerDeletedSuccessfully()
        {
            int id = 1;
            var deletePartnerQuery = new DeletePartnerQuery(id);
            var partner = new Partner { Id = 1, Title = "Title 1", LogoId = 1, 
                IsKeyPartner = true, IsVisibleEverywhere = true,
            };
            var dto = new PartnerDTO { Id = 1, Title = "Title 1", LogoId = 1,
                IsKeyPartner = true, IsVisibleEverywhere = true,
            };

            _PartnersRepositoryMock
                .Setup(repo => repo.GetFirstOrDefaultAsync(
                    It.IsAny<Expression<Func<Partner, bool>>>(),
                    null))
                .ReturnsAsync(partner);

            _repositoryWrapperMock
                .Setup(wrapper => wrapper.SaveChangesAsync())
                .ReturnsAsync(1);

            _mapperMock
                .Setup(mapper => mapper.Map<PartnerDTO>(partner))
                .Returns(dto);

            var result = await _handler.Handle(deletePartnerQuery, CancellationToken.None);

            result.IsSuccess.Should().BeTrue();
            result.Value.Should().BeEquivalentTo(dto);

            _PartnersRepositoryMock.Verify(
                repo => repo.Delete(partner),
                Times.Once);

            _repositoryWrapperMock.Verify(
                wrapper => wrapper.SaveChanges(),
                Times.Once);

            _mapperMock.Verify(
                mapper => mapper.Map<PartnerDTO>(partner),
                Times.Once);

            _loggerMock.Verify(
                logger => logger.LogError(It.IsAny<object>(), It.IsAny<string>()),
                Times.Never);
        }

    }
}
