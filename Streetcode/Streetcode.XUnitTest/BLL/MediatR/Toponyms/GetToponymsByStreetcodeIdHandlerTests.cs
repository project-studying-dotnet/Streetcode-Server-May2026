namespace Streetcode.XUnitTest.BLL.MediatR.Toponyms
{
    using System.Linq.Expressions;
    using AutoMapper;
    using Streetcode.DAL.Entities.Streetcode;
    using FluentAssertions;
    using MockQueryable.Moq;
    using Moq;
    using Streetcode.BLL.DTO.AdditionalContent.Coordinates.Types;
    using Streetcode.BLL.DTO.AdditionalContent.Tag;
    using Streetcode.BLL.DTO.Streetcode;
    using Streetcode.BLL.DTO.Toponyms;
    using Streetcode.BLL.Interfaces.Logging;
    using Streetcode.BLL.MediatR.Toponyms.GetByStreetcodeId;
    using Streetcode.DAL.Entities.AdditionalContent;
    using Streetcode.DAL.Entities.AdditionalContent.Coordinates.Types;
    using Streetcode.DAL.Entities.Toponyms;
    using Streetcode.DAL.Repositories.Interfaces.Base;
    using Streetcode.DAL.Repositories.Interfaces.Toponyms;
    using Streetcode.DAL.Specifications.Base;
    using Xunit;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetToponymsByStreetcodeIdHandlerTests"/> class.
    /// </summary>
    public sealed class GetToponymsByStreetcodeIdHandlerTests
    {
        private readonly IMapper mapper;
        private readonly Mock<ILoggerService> loggerMock;
        private readonly Mock<IRepositoryWrapper> repositoryWrapperMock;
        private readonly Mock<IToponymRepository> toponymRepositoryMock;
        private readonly GetToponymsByStreetcodeIdHandler handler;

        /// <summary>
        /// Initializes a new instance of the <see cref="GetToponymsByStreetcodeIdHandlerTests"/> class.
        /// </summary>
        public GetToponymsByStreetcodeIdHandlerTests()
        {
            this.mapper = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Toponym, ToponymDTO>();
                cfg.CreateMap<ToponymCoordinate, ToponymCoordinateDTO>();
                cfg.CreateMap<StreetcodeContent, StreetcodeDTO>();
                cfg.CreateMap<Tag, StreetcodeTagDTO>();
            }).CreateMapper();
            this.loggerMock = new Mock<ILoggerService>();
            this.repositoryWrapperMock = new Mock<IRepositoryWrapper>();
            this.toponymRepositoryMock = new Mock<IToponymRepository>();
            this.repositoryWrapperMock.Setup(r => r.ToponymRepository).Returns(this.toponymRepositoryMock.Object);
            this.handler = new GetToponymsByStreetcodeIdHandler(this.repositoryWrapperMock.Object, this.mapper, this.loggerMock.Object);
        }

        /// <summary>
        /// Should return toponyms when found by streetcode id.
        /// </summary>
        /// <returns>Awaitable task.</returns>
        [Fact]
        public async Task Handle_ShouldReturnToponyms_WhenFound()
        {
            // Arrange
            IQueryable<Toponym> toponyms = new List<Toponym>()
            {
                new()
                {
                    Id = 1,
                    StreetName = "Шевченка",
                    Oblast = "Київська",
                    Streetcodes = new List<StreetcodeContent>()
                    {
                        new()
                        {
                            Id = 1,
                        },
                        new()
                        {
                            Id = 2,
                        },
                    },
                },
            }.BuildMock();
            List<ToponymDTO> expected_toponyms = new()
            {
                this.mapper.Map<ToponymDTO>(toponyms.First()),
            };
            GetToponymsByStreetcodeIdQuery query = new(1);
            this.toponymRepositoryMock.Setup(
                r => r.FindAll(It.IsAny<ISpecification<Toponym>>())
            ).Returns(toponyms);

            // Act
            var result = await this.handler.Handle(query, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().BeEquivalentTo(expected_toponyms);
            this.toponymRepositoryMock.Verify(
                r => r.FindAll(It.IsAny<ISpecification<Toponym>>()),
                Times.Once
            );
        }

        /// <summary>
        /// Should return toponyms with unique street name when found by streetcode id.
        /// </summary>
        /// <returns>Awaitable task.</returns>
        [Fact]
        public async Task Handle_ShouldReturnUniqueToponyms_WhenFound()
        {
            // Arrange
            IQueryable<Toponym> toponyms = new List<Toponym>()
            {
                new()
                {
                    Id = 1,
                    StreetName = "Шевченка",
                    Oblast = "Київська",
                    Streetcodes = new List<StreetcodeContent>()
                    {
                        new()
                        {
                            Id = 1,
                        },
                        new()
                        {
                            Id = 2,
                        },
                    },
                },
                new()
                {
                    Id = 2,
                    StreetName = "Шевченка",
                    Oblast = "Львівська",
                    Streetcodes = new List<StreetcodeContent>()
                    {
                        new()
                        {
                            Id = 1,
                        },
                        new()
                        {
                            Id = 2,
                        },
                    },
                },
            }.BuildMock();
            List<ToponymDTO> expected_toponyms = new()
            {
                this.mapper.Map<ToponymDTO>(toponyms.First()),
            };
            GetToponymsByStreetcodeIdQuery query = new(1);
            this.toponymRepositoryMock.Setup(
                r => r.FindAll(It.IsAny<ISpecification<Toponym>>())
            ).Returns(toponyms);

            // Act
            var result = await this.handler.Handle(query, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().BeEquivalentTo(expected_toponyms);
            this.toponymRepositoryMock.Verify(
                r => r.FindAll(It.IsAny<ISpecification<Toponym>>()),
                Times.Once
            );
        }

        /// <summary>
        /// Should return error and log it when toponyms not found by streetcode id.
        /// </summary>
        /// <returns>Awaitable task.</returns>
        [Fact]
        public async Task Handle_ShouldReturnError_WhenNotFound()
        {
            // Arrange
            GetToponymsByStreetcodeIdQuery query = new(3);
            this.toponymRepositoryMock.Setup(
                r => r.FindAll(It.IsAny<ISpecification<Toponym>>())
            ).Returns(Enumerable.Empty<Toponym>().BuildMock());
            this.loggerMock.Setup(
                l => l.LogError(query, It.IsAny<string>())
            );

            // Act
            var result = await this.handler.Handle(query, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Errors.Should().NotBeEmpty();
            this.toponymRepositoryMock.Verify(
                r => r.FindAll(It.IsAny<ISpecification<Toponym>>()),
                Times.Once
            );
            this.loggerMock.Verify(
                l => l.LogError(query, It.IsAny<string>()),
                Times.Once
            );
        }
    }
}