using AutoMapper;
using Castle.Core.Logging;
using Microsoft.EntityFrameworkCore.Query;
using Moq;
using Org.BouncyCastle.Asn1.Ocsp;
using Repositories.Interfaces;
using Streetcode.BLL.DTO.Media.Art;
using Streetcode.BLL.Interfaces.BlobStorage;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.MediatR.Media.Art.GetByStreetcodeId;
using Streetcode.DAL.Entities.Media.Images;
using Streetcode.DAL.Entities.Streetcode;
using Streetcode.DAL.Repositories.Interfaces.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Streetcode.XUnitTest.BLL.MediatR.Media.Art.GetByStreetcodeId
{
    /// <summary>
    /// Checking class GetArtByStreetcodeIdHandler.
    /// </summary>
    public class GetArtByStreetcodeIdHandlerTests
    {
        private GetArtsByStreetcodeIdHandler handler;

        private Mock<IRepositoryWrapper> mockRepository;
        private Mock<IMapper> mockMapper;
        private Mock<IBlobService> mockBlobService;
        private Mock<ILoggerService> mockLoggerService;
        private Mock<IArtRepository> mockArtRepository;

        private IMapper mapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="GetArtByStreetcodeIdHandlerTests"/> class.
        /// </summary>
        public GetArtByStreetcodeIdHandlerTests()
        {
            this.mockRepository = new Mock<IRepositoryWrapper>();
            this.mockMapper = new Mock<IMapper>();
            this.mockBlobService = new Mock<IBlobService>();
            this.mockLoggerService = new Mock<ILoggerService>();
            this.mockArtRepository = new Mock<IArtRepository>();

            this.mockRepository
                .Setup(r => r.ArtRepository)
                .Returns(this.mockArtRepository.Object);

            this.mapper = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<DAL.Entities.Media.Images.Art, ArtDTO>();
            }).CreateMapper();

            this.handler = new GetArtsByStreetcodeIdHandler(
                this.mockRepository.Object,
                this.mapper,
                this.mockBlobService.Object,
                this.mockLoggerService.Object);
        }

        /// <summary>
        /// Method returns correct arts, if entered valid streetcodeId.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task Handle_ValidStreetcodeId_ReturnCorrectArts()
        {
            var query = new GetArtsByStreetcodeIdQuery(1);

            var expectedCount = 1;

            var arts = new List<DAL.Entities.Media.Images.Art>()
            {
                new DAL.Entities.Media.Images.Art()
                {
                    Id = 1,
                    Description = "Description art 1",
                    ImageId = 1,
                    Title = "Title art 1",
                    StreetcodeArts = new List<StreetcodeArt>()
                    {
                        new StreetcodeArt()
                        {
                            ArtId = 1,
                            StreetcodeId = 1,
                        },
                    },
                },
            };

            this.mockArtRepository
                    .Setup(r => r.GetAllAsync(
                        It.IsAny<Expression<Func<DAL.Entities.Media.Images.Art, bool>>>(),
                        It.IsAny<Func<IQueryable<DAL.Entities.Media.Images.Art>,
                            IIncludableQueryable<DAL.Entities.Media.Images.Art, object>>>()))
                    .ReturnsAsync(arts);

            var result = await this.handler.Handle(query, CancellationToken.None);

            Assert.Equal(expectedCount, result.Value.Count());
        }

        /// <summary>
        /// Method returns error message, if streetcode not exist arts.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task Handle_ValidStretcodeIdWithEmptyArts_ReturnErrorMessage()
        {
            var query = new GetArtsByStreetcodeIdQuery(1);

            var expectedErrorMessage = $"Cannot find any art with corresponding streetcode id: {query.StreetcodeId}";

            this.mockArtRepository
                    .Setup(r => r.GetAllAsync(
                        It.IsAny<Expression<Func<DAL.Entities.Media.Images.Art, bool>>>(),
                        It.IsAny<Func<IQueryable<DAL.Entities.Media.Images.Art>,
                            IIncludableQueryable<DAL.Entities.Media.Images.Art, object>>>()))
                    .ReturnsAsync((List<DAL.Entities.Media.Images.Art>?)null);

            var result = await this.handler.Handle(query, CancellationToken.None);

            Assert.Equal(expectedErrorMessage, result.Errors.First().Message);
        }
    }
}
