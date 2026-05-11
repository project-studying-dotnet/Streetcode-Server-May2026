namespace Streetcode.XUnitTest.BLL.MediatR.Toponyms
{
    using AutoMapper;
    using Moq;
    using Streetcode.BLL.DTO.Toponyms;
    using Streetcode.BLL.Interfaces.Logging;
    using Streetcode.BLL.MediatR.Toponyms.GetById;
    using Streetcode.DAL.Entities.Toponyms;
    using Streetcode.DAL.Repositories.Interfaces.Base;
    using Streetcode.DAL.Repositories.Interfaces.Toponyms;
    using Xunit;
    using DAL.Entities.Streetcode;
    using Streetcode.BLL.MediatR.Toponyms.GetByStreetcodeId;
    using System.Linq.Expressions;
    using Microsoft.EntityFrameworkCore.Query;
    using Streetcode.BLL.DTO.Streetcode;
    using FluentAssertions;

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
                cfg.CreateMap<StreetcodeContent, StreetcodeDTO>();
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
            List<Toponym> toponyms = new()
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
            };
            List<ToponymDTO> expected_toponyms = new()
            {
                this.mapper.Map<ToponymDTO>(toponyms[0]),
            };
            GetToponymsByStreetcodeIdQuery query = new(1);
            this.toponymRepositoryMock.Setup(
                r => r.GetAllAsync(
                    It.IsAny<Expression<Func<Toponym, bool>>>(),
                    It.IsAny<Func<IQueryable<Toponym>, IIncludableQueryable<Toponym, object>>>()
                )
            ).ReturnsAsync(toponyms);

            // Act
            var result = await this.handler.Handle(query, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().BeEquivalentTo(expected_toponyms);
            this.toponymRepositoryMock.Verify(
                r => r.GetAllAsync(
                    It.IsAny<Expression<Func<Toponym, bool>>>(),
                    It.IsAny<Func<IQueryable<Toponym>, IIncludableQueryable<Toponym, object>>>()
                ),
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
            List<Toponym> toponyms = new()
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
            };
            List<ToponymDTO> expected_toponyms = new()
            {
                this.mapper.Map<ToponymDTO>(toponyms[0]),
            };
            GetToponymsByStreetcodeIdQuery query = new(1);
            this.toponymRepositoryMock.Setup(
                r => r.GetAllAsync(
                    It.IsAny<Expression<Func<Toponym, bool>>>(),
                    It.IsAny<Func<IQueryable<Toponym>, IIncludableQueryable<Toponym, object>>>()
                )
            ).ReturnsAsync(toponyms);

            // Act
            var result = await this.handler.Handle(query, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().BeEquivalentTo(expected_toponyms);
            this.toponymRepositoryMock.Verify(
                r => r.GetAllAsync(
                    It.IsAny<Expression<Func<Toponym, bool>>>(),
                    It.IsAny<Func<IQueryable<Toponym>, IIncludableQueryable<Toponym, object>>>()
                ),
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
                r => r.GetAllAsync(
                    It.IsAny<Expression<Func<Toponym, bool>>>(),
                    It.IsAny<Func<IQueryable<Toponym>, IIncludableQueryable<Toponym, object>>>()
                )
            ).ReturnsAsync(Enumerable.Empty<Toponym>());
            this.loggerMock.Setup(
                l => l.LogError(query, It.IsAny<string>())
            );

            // Act
            var result = await this.handler.Handle(query, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Errors.Should().NotBeEmpty();
            this.toponymRepositoryMock.Verify(
                r => r.GetAllAsync(
                    It.IsAny<Expression<Func<Toponym, bool>>>(),
                    It.IsAny<Func<IQueryable<Toponym>, IIncludableQueryable<Toponym, object>>>()
                ),
                Times.Once
            );
            this.loggerMock.Verify(
                l => l.LogError(query, It.IsAny<string>()),
                Times.Once
            );
        }
    }
}