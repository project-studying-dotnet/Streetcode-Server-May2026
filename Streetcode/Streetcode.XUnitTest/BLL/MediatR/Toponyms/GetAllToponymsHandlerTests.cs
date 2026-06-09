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

namespace Streetcode.XUnitTest.BLL.MediatR.Toponyms;

public sealed class GetAllToponymsHandlerTests
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
    private readonly IMapper _mapper;
    private readonly Mock<ILoggerService> _loggerMock;
    private readonly Mock<IRepositoryWrapper> _repositoryWrapperMock;
    private readonly Mock<IToponymRepository> _toponymRepositoryMock;
    private readonly GetAllToponymsHandler _handler;

    public GetAllToponymsHandlerTests()
    {
        _mapper = new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<Toponym, ToponymDTO>();
        }).CreateMapper();

        _loggerMock = new Mock<ILoggerService>();
        _repositoryWrapperMock = new Mock<IRepositoryWrapper>();
        _toponymRepositoryMock = new Mock<IToponymRepository>();

        _repositoryWrapperMock
            .Setup(r => r.ToponymRepository)
            .Returns(_toponymRepositoryMock.Object);

        _handler = new GetAllToponymsHandler(
            _repositoryWrapperMock.Object,
            _mapper,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnAllToponyms_WhenNoTitleProvided()
    {
        IQueryable<Toponym> toponyms = new List<Toponym>
        {
            new()
            {
                Id = 1,
                StreetName = "Шевченка",
            },
            new()
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
            new()
            {
                Id = 1,
                StreetName = "Шевченка",
            },
            new()
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
