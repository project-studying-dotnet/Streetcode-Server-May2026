namespace Streetcode.XUnitTest.BLL.MediatR.Toponyms
{
    using System;
    using System.Linq.Expressions;
    using AutoMapper;
    using FluentAssertions;
    using Moq;
    using Streetcode.BLL.DTO.Toponyms;
    using Streetcode.BLL.Interfaces.Logging;
    using Streetcode.BLL.MediatR.Toponyms.GetAll;
    using Streetcode.DAL.Entities.Toponyms;
    using Streetcode.DAL.Repositories.Interfaces.Base;
    using Streetcode.DAL.Repositories.Interfaces.Toponyms;
    using Xunit;

    /// <summary>
    /// Contains tests for <see cref="GetAllToponymsHandler"/>.
    /// </summary>
    public sealed class GetAllToponymsHandlerTests
    {
        private readonly IMapper mapper;
        private readonly Mock<ILoggerService> loggerMock;
        private readonly Mock<IRepositoryWrapper> repositoryWrapperMock;
        private readonly Mock<IToponymRepository> toponymRepositoryMock;
        private readonly GetAllToponymsHandler handler;

        /// <summary>
        /// Initializes a new instance of the <see cref="GetAllToponymsHandlerTests"/> class.
        /// </summary>
        public GetAllToponymsHandlerTests()
        {
            this.mapper = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Toponym, ToponymDTO>();
            }).CreateMapper();
            this.loggerMock = new Mock<ILoggerService>();
            this.repositoryWrapperMock = new Mock<IRepositoryWrapper>();
            this.toponymRepositoryMock = new Mock<IToponymRepository>();
            this.repositoryWrapperMock.Setup(r => r.ToponymRepository).Returns(this.toponymRepositoryMock.Object);
            this.handler = new GetAllToponymsHandler(this.repositoryWrapperMock.Object, this.mapper, this.loggerMock.Object);
        }

        /// <summary>
        /// Should return all toponyms when <see cref="GetAllToponymsRequestDTO.Title"/> <see langword="is null"/>.
        /// </summary>
        /// <returns>Awaitable task.</returns>
        [Fact]
        public async Task Handle_ShouldReturnAllToponyms_WhenNoTitleProvided()
        {
            // Arrange
            IQueryable<Toponym> toponyms = new List<Toponym>
            {
                new()
                {
                    Id = 1,
                    StreetName = "Шевченка",
                },
                new()
                {
                    Id = 2,
                    StreetName = "Бандери",
                },
            }.AsQueryable();
            List<ToponymDTO> expected_toponyms = new()
            {
                this.mapper.Map<ToponymDTO>(toponyms.ElementAt(0)),
                this.mapper.Map<ToponymDTO>(toponyms.ElementAt(1)),
            };
            GetAllToponymsQuery query = new(new GetAllToponymsRequestDTO
            {
                Title = null,
            });
            toponymRepositoryMock.Setup(r => r.FindAll((Expression<Func<Toponym, bool>>?)null)).Returns(toponyms);

            // Act
            var result = await this.handler.Handle(query, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Toponyms.Should().BeEquivalentTo(expected_toponyms);
            toponymRepositoryMock.Verify(r => r.FindAll((Expression<Func<Toponym, bool>>?)null), Times.Once);
        }

        /// <summary>
        /// Should return filtered toponyms when <see cref="GetAllToponymsRequestDTO.Title"/> <see langword="is not null"/>.
        /// Must be case-insensitive.
        /// </summary>
        /// <returns>Awaitable task.</returns>
        [Fact]
        public async Task Handle_ShouldFilterToponyms_WhenTitleProvided_CaseInsensitive()
        {
            // Arrange
            IQueryable<Toponym> toponyms = new List<Toponym>
            {
                new()
                {
                    Id = 1,
                    StreetName = "Шевченка",
                },
                new()
                {
                    Id = 2,
                    StreetName = "Бандери",
                },
            }.AsQueryable();
            List<ToponymDTO> expected_toponyms = new()
            {
                this.mapper.Map<ToponymDTO>(toponyms.ElementAt(1)),
            };
            GetAllToponymsQuery query = new(new GetAllToponymsRequestDTO
            {
                Title = "аНдЕр",
            });
            toponymRepositoryMock.Setup(r => r.FindAll((Expression<Func<Toponym, bool>>?)null)).Returns(toponyms);

            // Act
            var result = await this.handler.Handle(query, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Toponyms.Should().BeEquivalentTo(expected_toponyms);
            this.toponymRepositoryMock.Verify(r => r.FindAll((Expression<Func<Toponym, bool>>?)null), Times.Once);
        }

        /// <summary>
        /// Should return unique toponyms when <see cref="GetAllToponymsRequestDTO.Title"/> <see langword="is not null"/>.
        /// </summary>
        /// <returns>Awaitable task.</returns>
        [Fact]
        public async Task Handle_ShouldReturnUniqueToponyms_WhenTitleProvided()
        {
            // Arrange
            IQueryable<Toponym> toponyms = new List<Toponym>
            {
                new()
                {
                    Id = 1,
                    StreetName = "Шевченка",
                    Oblast = "Київська",
                },
                new()
                {
                    Id = 2,
                    StreetName = "Шевченка",
                    Oblast = "Львівська",
                },
            }.AsQueryable();
            List<ToponymDTO> expected_toponyms = new()
            {
                this.mapper.Map<ToponymDTO>(toponyms.ElementAt(0)),
            };
            GetAllToponymsQuery query = new(new GetAllToponymsRequestDTO
            {
                Title = "евч",
            });
            toponymRepositoryMock.Setup(r => r.FindAll((Expression<Func<Toponym, bool>>?)null)).Returns(toponyms);

            // Act
            var result = await this.handler.Handle(query, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Toponyms.Should().BeEquivalentTo(expected_toponyms);
            this.toponymRepositoryMock.Verify(r => r.FindAll((Expression<Func<Toponym, bool>>?)null), Times.Once);
        }

        /// <summary>
        /// Should return empty collection when no matches found.
        /// </summary>
        /// <returns>Awaitable task.</returns>
        [Fact]
        public async Task Handle_ShouldReturnEmptyCollection_WhenNoMatchesFound()
        {
            // Arrange
            IQueryable<Toponym> toponyms = new List<Toponym>
            {
                new()
                {
                    Id = 1,
                    StreetName = "Шевченка",
                },
                new()
                {
                    Id = 2,
                    StreetName = "Бандери",
                },
            }.AsQueryable();
            List<ToponymDTO> expected_toponyms = new();
            GetAllToponymsQuery query = new(new GetAllToponymsRequestDTO
            {
                Title = "ийськ",
            });
            toponymRepositoryMock.Setup(r => r.FindAll((Expression<Func<Toponym, bool>>?)null)).Returns(toponyms);

            // Act
            var result = await this.handler.Handle(query, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Toponyms.Should().BeEquivalentTo(expected_toponyms);
            this.toponymRepositoryMock.Verify(r => r.FindAll((Expression<Func<Toponym, bool>>?)null), Times.Once);
        }
    }
}