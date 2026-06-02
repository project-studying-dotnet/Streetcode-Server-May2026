using System.Linq.Expressions;
using AutoMapper;
using FluentAssertions;
using Moq;
using Streetcode.BLL.DTO.Transactions;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.Resources;
using Streetcode.BLL.MediatR.Transactions.TransactionLink.GetById;
using Streetcode.DAL.Repositories.Interfaces.Base;
using Xunit;

using TransactionLinkEntity = Streetcode.DAL.Entities.Transactions.TransactionLink;

namespace Streetcode.XUnitTest.MediatRTests.Transactions.TransactionLink.GetById;

public class GetTransactLinkByIdHandlerTests
{
    private readonly Mock<IRepositoryWrapper> _repositoryWrapperMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<ILoggerService> _loggerMock;
    private readonly GetTransactLinkByIdHandler _handler;

    public GetTransactLinkByIdHandlerTests()
    {
        _repositoryWrapperMock = new Mock<IRepositoryWrapper>();
        _mapperMock = new Mock<IMapper>();
        _loggerMock = new Mock<ILoggerService>();

        _handler = new GetTransactLinkByIdHandler(
            _repositoryWrapperMock.Object,
            _mapperMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnOkResult_WithTransactLink_WhenTransactLinkExists()
    {
        // Arrange
        const int transactLinkId = 1;
        var query = new GetTransactLinkByIdQuery(transactLinkId);

        var transactLink = new TransactionLinkEntity
        {
            Id = transactLinkId
        };

        var transactLinkDto = new TransactLinkDTO
        {
            Id = transactLinkId
        };

        _repositoryWrapperMock
            .Setup(x => x.TransactLinksRepository.GetFirstOrDefaultAsync(
                It.IsAny<Expression<Func<TransactionLinkEntity, bool>>>(),
                null))
            .ReturnsAsync(transactLink);

        _mapperMock
            .Setup(x => x.Map<TransactLinkDTO>(transactLink))
            .Returns(transactLinkDto);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEquivalentTo(transactLinkDto);

        _repositoryWrapperMock.Verify(
            x => x.TransactLinksRepository.GetFirstOrDefaultAsync(
                It.IsAny<Expression<Func<TransactionLinkEntity, bool>>>(),
                null),
            Times.Once);

        _mapperMock.Verify(
            x => x.Map<TransactLinkDTO>(transactLink),
            Times.Once);

        _loggerMock.Verify(
            x => x.LogError(It.IsAny<object>(), It.IsAny<string>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldReturnFailResult_AndLogError_WhenTransactLinkDoesNotExist()
    {
        // Arrange
        const int transactLinkId = 1;
        var query = new GetTransactLinkByIdQuery(transactLinkId);
        string expectedErrorMessage = string.Format(ErrorMessages.CannotFindAnyTransactionLinkWithCorrespondingId, transactLinkId);

        _repositoryWrapperMock
            .Setup(x => x.TransactLinksRepository.GetFirstOrDefaultAsync(
                It.IsAny<Expression<Func<TransactionLinkEntity, bool>>>(),
                null))
            .ReturnsAsync((TransactionLinkEntity?)null);

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
            x => x.Map<TransactLinkDTO>(It.IsAny<TransactionLinkEntity>()),
            Times.Never);
    }
}