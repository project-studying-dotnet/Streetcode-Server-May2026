using AutoMapper;
using Microsoft.EntityFrameworkCore.Query;
using Moq;
using Org.BouncyCastle.Asn1.Ocsp;
using Repositories.Interfaces;
using Streetcode.BLL.DTO.Media.Art;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.MediatR.Media.Art.GetById;
using Streetcode.DAL.Repositories.Interfaces.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Streetcode.XUnitTest.BLL.MediatR.Media.Art.GetById
{
    /// <summary>
    /// Checking class GetArtByIdHandler.
    /// </summary>
    public class GetArtByIdHandlerTests
    {
        private GetArtByIdHandler handler;

        private Mock<IMapper> mockMapper;
        private Mock<IRepositoryWrapper> mockRepository;
        private Mock<IArtRepository> mockArtRepository;
        private Mock<ILoggerService> mockLoggerService;

        /// <summary>
        /// Initializes a new instance of the <see cref="GetArtByIdHandlerTests"/> class.
        /// </summary>
        public GetArtByIdHandlerTests()
        {
            this.mockMapper = new Mock<IMapper>();
            this.mockArtRepository = new Mock<IArtRepository>();
            this.mockRepository = new Mock<IRepositoryWrapper>();
            this.mockLoggerService = new Mock<ILoggerService>();

            this.mockRepository
                .Setup(r => r.ArtRepository)
                .Returns(this.mockArtRepository.Object);

            this.handler = new GetArtByIdHandler(
                    this.mockRepository.Object,
                    this.mockMapper.Object,
                    this.mockLoggerService.Object);
        }

        /// <summary>
        /// Method returns correct art, if entered valid id.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task Handle_ValidId_ReturnArt()
        {
            var query = new GetArtByIdQuery(1);

            var art = new DAL.Entities.Media.Images.Art()
            {
                Id = 1,
                Description = "Description art 1",
                ImageId = 1,
                Title = "Title art 1",
            };

            var artDTO = new ArtDTO()
            {
                Id = 1,
                Description = "Description art 1",
                ImageId = 1,
                Title = "Title art 1",
            };

            this.mockArtRepository
                    .Setup(r => r.GetFirstOrDefaultAsync(
                        It.IsAny<Expression<Func<DAL.Entities.Media.Images.Art, bool>>>(),
                        It.IsAny<Func<IQueryable<DAL.Entities.Media.Images.Art>,
                            IIncludableQueryable<DAL.Entities.Media.Images.Art, object>>>()))
                    .ReturnsAsync(art);

            this.mockMapper
                    .Setup(m => m.Map<ArtDTO>(
                        It.IsAny<DAL.Entities.Media.Images.Art>()))
                    .Returns(artDTO);

            var result = await this.handler.Handle(query, CancellationToken.None);

            Assert.Equal(artDTO.Id, result.Value.Id);
        }

        /// <summary>
        /// Method returns error message, if entered not valid id.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task Handle_NotValidId_ReturnError()
        {
            var query = new GetArtByIdQuery(2);

            string expectedErrorMsg = $"Cannot find an art with corresponding id: {query.Id}";

            this.mockArtRepository
                    .Setup(r => r.GetFirstOrDefaultAsync(
                        It.IsAny<Expression<Func<DAL.Entities.Media.Images.Art, bool>>>(),
                        It.IsAny<Func<IQueryable<DAL.Entities.Media.Images.Art>,
                            IIncludableQueryable<DAL.Entities.Media.Images.Art, object>>>()))
                    .ReturnsAsync((DAL.Entities.Media.Images.Art?)null);

            var result = await this.handler.Handle(query, CancellationToken.None);

            Assert.Equal(expectedErrorMsg, result.Errors.First().Message);
        }
    }
}
