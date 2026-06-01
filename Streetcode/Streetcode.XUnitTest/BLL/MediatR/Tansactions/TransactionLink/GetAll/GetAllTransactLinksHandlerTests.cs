using AutoMapper;
using FluentAssertions;
using Moq;
using Streetcode.BLL.DTO.Transactions;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.MediatR.Transactions.TransactionLink.GetAll;
using Streetcode.BLL.Resources;
using Streetcode.DAL.Repositories.Interfaces.Base;
using Xunit;

using TransactionLinkEntity = Streetcode.DAL.Entities.Transactions.TransactionLink;

namespace Streetcode.XUnitTest.MediatRTests.Transactions.TransactionLink.GetAll;

public class GetAllTransactLinksHandlerTests
{
    private readonly Mock<IRepositoryWrapper> _repositoryWrapperMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<ILoggerService> _loggerMock;
    private readonly GetAllTransactLinksHandler _handler;

    public GetAllTransactLinksHandlerTests()
    {
        _repositoryWrapperMock = new Mock<IRepositoryWrapper>();
        _mapperMock = new Mock<IMapper>();
        _loggerMock = new Mock<ILoggerService>();

        _handler = new GetAllTransactLinksHandler(
            _repositoryWrapperMock.Object,
            _mapperMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnOkResult_WithTransactLinks_WhenTransactLinksExist()
    {
        // Arrange
        var query = new GetAllTransactLinksQuery();

        var transactLinks = new List<TransactionLinkEntity>
        {
            new TransactionLinkEntity { Id = 1 },
            new TransactionLinkEntity { Id = 2 }
        };

        var transactLinkDtos = new List<TransactLinkDTO>
        {
            new TransactLinkDTO { Id = 1 },
            new TransactLinkDTO { Id = 2 }
        };

        _repositoryWrapperMock
            .Setup(x => x.TransactLinksRepository.GetAllAsync())
            .ReturnsAsync(transactLinks);

        _mapperMock
            .Setup(x => x.Map<IEnumerable<TransactLinkDTO>>(transactLinks))
            .Returns(transactLinkDtos);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEquivalentTo(transactLinkDtos);

        _repositoryWrapperMock.Verify(
            x => x.TransactLinksRepository.GetAllAsync(),
            Times.Once);

        _mapperMock.Verify(
            x => x.Map<IEnumerable<TransactLinkDTO>>(transactLinks),
            Times.Once);

        _loggerMock.Verify(
            x => x.LogError(It.IsAny<object>(), It.IsAny<string>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldReturnFailResult_AndLogError_WhenTransactLinksAreNull()
    {
        // Arrange
        var query = new GetAllTransactLinksQuery();
        string expectedErrorMessage = ErrorMessages.CannotFindAnyTransactionLink;

        _repositoryWrapperMock
            .Setup(x => x.TransactLinksRepository.GetAllAsync())
            .ReturnsAsync((IEnumerable<TransactionLinkEntity>?)null!);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainSingle();
        result.Errors[0].Message.Should().Be(expectedErrorMessage);

        _loggerMock.Verify(
            x => x.LogError(query, expectedErrorMessage),
            Times.Once);

        _mapperMock.Verify(
            x => x.Map<IEnumerable<TransactLinkDTO>>(It.IsAny<IEnumerable<TransactionLinkEntity>>()),
            Times.Never);
    }
}