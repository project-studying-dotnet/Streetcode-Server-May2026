namespace Streetcode.XUnitTest.BLL.MediatR.Toponyms
{
    using System.Linq.Expressions;
    using AutoMapper;
    using FluentAssertions;
    using Moq;
    using Streetcode.BLL.DTO.Toponyms;
    using Streetcode.BLL.Interfaces.Logging;
    using Streetcode.BLL.MediatR.Toponyms.GetById;
    using Streetcode.DAL.Entities.Toponyms;
    using Streetcode.DAL.Repositories.Interfaces.Base;
    using Streetcode.DAL.Repositories.Interfaces.Toponyms;
    using Xunit;

    /// <summary>
    /// Contains tests for <see cref="GetToponymByIdHandler"/>.
    /// </summary>
    public sealed class GetToponymByIdHandlerTests
    {
        private readonly IMapper mapper;
        private readonly Mock<ILoggerService> loggerMock;
        private readonly Mock<IRepositoryWrapper> repositoryWrapperMock;
        private readonly Mock<IToponymRepository> toponymRepositoryMock;
        private readonly GetToponymByIdHandler handler;

        /// <summary>
        /// Initializes a new instance of the <see cref="GetToponymByIdHandlerTests"/> class.
        /// </summary>
        public GetToponymByIdHandlerTests()
        {
            this.mapper = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Toponym, ToponymDTO>();
            }).CreateMapper();
            this.loggerMock = new Mock<ILoggerService>();
            this.repositoryWrapperMock = new Mock<IRepositoryWrapper>();
            this.toponymRepositoryMock = new Mock<IToponymRepository>();
            this.repositoryWrapperMock.Setup(r => r.ToponymRepository).Returns(this.toponymRepositoryMock.Object);
            this.handler = new GetToponymByIdHandler(this.repositoryWrapperMock.Object, this.mapper, this.loggerMock.Object);
        }

        /// <summary>
        /// Should return toponym when found by id.
        /// </summary>
        /// <returns>Awaitable task.</returns>
        [Fact]
        public async Task Handle_ShouldReturnToponym_WhenFound()
        {
            // Arrange
            Toponym toponym = new()
            {
                Id = 2,
                StreetName = "Бандери",
            };
            ToponymDTO expected_toponym = this.mapper.Map<ToponymDTO>(toponym);
            GetToponymByIdQuery query = new(2);
            this.toponymRepositoryMock.Setup(
                r => r.GetFirstOrDefaultAsync(
                    It.IsAny<Expression<Func<Toponym, bool>>>(),
                    null
                )
            ).ReturnsAsync(toponym);

            // Act
            var result = await this.handler.Handle(query, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().BeEquivalentTo(expected_toponym);
            this.toponymRepositoryMock.Verify(
                r => r.GetFirstOrDefaultAsync(
                    It.IsAny<Expression<Func<Toponym, bool>>>(),
                    null
                ),
                Times.Once
            );
        }

        /// <summary>
        /// Should return error and log it when toponym not found by id.
        /// </summary>
        /// <returns>Awaitable task.</returns>
        [Fact]
        public async Task Handle_ShouldReturnError_WhenNotFound()
        {
            // Arrange
            GetToponymByIdQuery query = new(1);
            this.toponymRepositoryMock.Setup(
                r => r.GetFirstOrDefaultAsync(
                    It.IsAny<Expression<Func<Toponym, bool>>>(),
                    null
                )
            ).ReturnsAsync(null as Toponym);
            this.loggerMock.Setup(
                l => l.LogError(query, It.IsAny<string>())
            );

            // Act
            var result = await this.handler.Handle(query, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Errors.Should().NotBeEmpty();
            this.toponymRepositoryMock.Verify(
                r => r.GetFirstOrDefaultAsync(
                    It.IsAny<Expression<Func<Toponym, bool>>>(),
                    null
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