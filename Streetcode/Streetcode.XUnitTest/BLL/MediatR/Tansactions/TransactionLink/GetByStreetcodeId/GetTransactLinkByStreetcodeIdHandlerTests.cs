using System.Linq.Expressions;
using AutoMapper;
using FluentAssertions;
using Moq;
using Streetcode.BLL.DTO.Transactions;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.MediatR.Transactions.TransactionLink.GetByStreetcodeId;
using Streetcode.BLL.Resources;
using Streetcode.DAL.Repositories.Interfaces.Base;
using Xunit;

using StreetcodeEntity = Streetcode.DAL.Entities.Streetcode.StreetcodeContent;
using TransactionLinkEntity = Streetcode.DAL.Entities.Transactions.TransactionLink;

namespace Streetcode.XUnitTest.MediatRTests.Transactions.TransactionLink.GetByStreetcodeId;

public class GetTransactLinkByStreetcodeIdHandlerTests
{
    private readonly Mock<IRepositoryWrapper> _repositoryWrapperMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<ILoggerService> _loggerMock;
    private readonly GetTransactLinkByStreetcodeIdHandler _handler;

    public GetTransactLinkByStreetcodeIdHandlerTests()
    {
        _repositoryWrapperMock = new Mock<IRepositoryWrapper>();
        _mapperMock = new Mock<IMapper>();
        _loggerMock = new Mock<ILoggerService>();

        _handler = new GetTransactLinkByStreetcodeIdHandler(
            _repositoryWrapperMock.Object,
            _mapperMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnOkResult_WithTransactLink_WhenTransactLinkExists()
    {
        // Arrange
        const int streetcodeId = 1;
        var query = new GetTransactLinkByStreetcodeIdQuery(streetcodeId);

        var transactLink = new TransactionLinkEntity
        {
            Id = 10,
            StreetcodeId = streetcodeId
        };

        var transactLinkDto = new TransactLinkDTO
        {
            Id = 10,
            StreetcodeId = streetcodeId
        };

        _repositoryWrapperMock
            .Setup(x => x.TransactLinksRepository.GetFirstOrDefaultAsync(
                It.IsAny<Expression<Func<TransactionLinkEntity, bool>>>(),
                null))
            .ReturnsAsync(transactLink);

        _mapperMock
            .Setup(x => x.Map<TransactLinkDTO?>(transactLink))
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

        _repositoryWrapperMock.Verify(
            x => x.StreetcodeRepository.GetFirstOrDefaultAsync(
                It.IsAny<Expression<Func<StreetcodeEntity, bool>>>(),
                null),
            Times.Never);

        _mapperMock.Verify(
            x => x.Map<TransactLinkDTO?>(transactLink),
            Times.Once);

        _loggerMock.Verify(
            x => x.LogError(It.IsAny<object>(), It.IsAny<string>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldReturnOkResult_WithNullValue_WhenTransactLinkDoesNotExist_ButStreetcodeExists()
    {
        // Arrange
        const int streetcodeId = 1;
        var query = new GetTransactLinkByStreetcodeIdQuery(streetcodeId);

        var streetcode = new StreetcodeEntity
        {
            Id = streetcodeId
        };

        _repositoryWrapperMock
            .Setup(x => x.TransactLinksRepository.GetFirstOrDefaultAsync(
                It.IsAny<Expression<Func<TransactionLinkEntity, bool>>>(),
                null))
            .ReturnsAsync((TransactionLinkEntity?)null);

        _repositoryWrapperMock
            .Setup(x => x.StreetcodeRepository.GetFirstOrDefaultAsync(
                It.IsAny<Expression<Func<StreetcodeEntity, bool>>>(),
                null))
            .ReturnsAsync(streetcode);

        _mapperMock
            .Setup(x => x.Map<TransactLinkDTO?>(It.IsAny<TransactionLinkEntity>()))
            .Returns((TransactLinkDTO?)null);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeNull();

        _loggerMock.Verify(
            x => x.LogError(It.IsAny<object>(), It.IsAny<string>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldReturnFailResult_AndLogError_WhenTransactLinkAndStreetcodeDoNotExist()
    {
        // Arrange
        const int streetcodeId = 1;
        var query = new GetTransactLinkByStreetcodeIdQuery(streetcodeId);
        string expectedErrorMessage = string.Format(
            ErrorMessages.CannotFindAnyTransactionLinkWithCorrespondingStreetcodeId,
            streetcodeId);

        _repositoryWrapperMock
            .Setup(x => x.TransactLinksRepository.GetFirstOrDefaultAsync(
                It.IsAny<Expression<Func<TransactionLinkEntity, bool>>>(),
                null))
            .ReturnsAsync((TransactionLinkEntity?)null);

        _repositoryWrapperMock
            .Setup(x => x.StreetcodeRepository.GetFirstOrDefaultAsync(
                It.IsAny<Expression<Func<StreetcodeEntity, bool>>>(),
                null))
            .ReturnsAsync((StreetcodeEntity?)null);

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
            x => x.Map<TransactLinkDTO?>(It.IsAny<TransactionLinkEntity>()),
            Times.Never);
    }
}
