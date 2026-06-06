// <copyright file="DeleteTextHandlerTests.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
using System.Linq.Expressions;
using AutoMapper;
using FluentAssertions;
using Moq;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.Mapping.Streetcode.TextContent;
using Streetcode.BLL.MediatR.Streetcode.Text.Delete;
using Streetcode.DAL.Repositories.Interfaces.Base;
using Streetcode.DAL.Repositories.Interfaces.Streetcode.TextContent;
using Xunit;

using TextEntity = Streetcode.DAL.Entities.Streetcode.TextContent.Text;

namespace Streetcode.XUnitTest.BLL.MediatR.StreetCode.Text.Delete;

public class DeleteTextHandlerTests
{
    private const int TextId = 1;
    private const string TextTitle = "Test Title";
    private const string DatabaseFailureMessage = "Database connection failure";

    private readonly Mock<IRepositoryWrapper> _repoWrapperMock;
    private readonly Mock<ITextRepository> _textRepoMock;
    private readonly IMapper _mapper;
    private readonly Mock<ILoggerService> _loggerMock;
    private readonly DeleteTextHandler _handler;

    public DeleteTextHandlerTests()
    {
        _mapper = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<TextProfile>();
        }).CreateMapper();

        _repoWrapperMock = new Mock<IRepositoryWrapper>();
        _textRepoMock = new Mock<ITextRepository>();
        _loggerMock = new Mock<ILoggerService>();

        _repoWrapperMock
            .Setup(x => x.TextRepository)
            .Returns(_textRepoMock.Object);

        _handler = new DeleteTextHandler(
            _repoWrapperMock.Object,
            _mapper,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnTextDto_WhenDeleteSucceeds()
    {
        var command = new DeleteTextCommand(TextId);
        var existingText = new TextEntity
        {
            Id = TextId,
            Title = TextTitle,
        };

        _textRepoMock
            .Setup(r => r.GetFirstOrDefaultAsync(
                It.IsAny<Expression<Func<TextEntity, bool>>>(),
                null))
            .ReturnsAsync(existingText);

        _textRepoMock
            .Setup(r => r.Delete(It.IsAny<TextEntity>()));

        _repoWrapperMock
            .Setup(w => w.SaveChangesAsync())
            .ReturnsAsync(1);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();

        _textRepoMock.Verify(
            r => r.GetFirstOrDefaultAsync(
                It.IsAny<Expression<Func<TextEntity, bool>>>(),
                null),
            Times.Once);

        _textRepoMock.Verify(
            r => r.Delete(It.IsAny<TextEntity>()),
            Times.Once);

        _repoWrapperMock.Verify(
            w => w.SaveChangesAsync(),
            Times.Once);

        _loggerMock.Verify(
            l => l.LogError(It.IsAny<object>(), It.IsAny<string>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldReturnFailResult_WhenDeleteFails()
    {
        var command = new DeleteTextCommand(TextId);
        var existingText = new TextEntity
        {
            Id = TextId,
            Title = TextTitle,
        };

        var expectedErrorMessage = $"Failed to delete Text with Id {TextId}.";

        _textRepoMock
            .Setup(r => r.GetFirstOrDefaultAsync(
                It.IsAny<Expression<Func<TextEntity, bool>>>(),
                null))
            .ReturnsAsync(existingText);

        _textRepoMock
            .Setup(r => r.Delete(It.IsAny<TextEntity>()));

        _repoWrapperMock
            .Setup(w => w.SaveChangesAsync())
            .ReturnsAsync(0);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsFailed.Should().BeTrue();

        result.Errors
            .Should()
            .ContainSingle()
            .Which.Message
            .Should()
            .Be(expectedErrorMessage);

        _textRepoMock.Verify(
            r => r.GetFirstOrDefaultAsync(
                It.IsAny<Expression<Func<TextEntity, bool>>>(),
                null),
            Times.Once);

        _textRepoMock.Verify(
            r => r.Delete(It.IsAny<TextEntity>()),
            Times.Once);

        _repoWrapperMock.Verify(
            w => w.SaveChangesAsync(),
            Times.Once);

        _loggerMock.Verify(
            l => l.LogError(
                It.IsAny<DeleteTextCommand>(),
                expectedErrorMessage),
            Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldReturnFail_WhenTextDoesNotExist()
    {
        var command = new DeleteTextCommand(TextId);
        var expectedErrorMessage = $"Text with Id {TextId} not found.";

        _textRepoMock
            .Setup(r => r.GetFirstOrDefaultAsync(
                It.IsAny<Expression<Func<TextEntity, bool>>>(),
                null))
            .ReturnsAsync((TextEntity)null!);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsFailed.Should().BeTrue();

        result.Errors
            .Should()
            .ContainSingle()
            .Which.Message
            .Should()
            .Be(expectedErrorMessage);

        _textRepoMock.Verify(
            r => r.GetFirstOrDefaultAsync(
                It.IsAny<Expression<Func<TextEntity, bool>>>(),
                null),
            Times.Once);

        _textRepoMock.Verify(
            r => r.Delete(It.IsAny<TextEntity>()),
            Times.Never);

        _repoWrapperMock.Verify(
            w => w.SaveChangesAsync(),
            Times.Never);

        _loggerMock.Verify(
            l => l.LogError(
                It.IsAny<DeleteTextCommand>(),
                expectedErrorMessage),
            Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldPropagateException_WhenRepositoryThrows()
    {
        var command = new DeleteTextCommand(TextId);

        _textRepoMock
            .Setup(r => r.GetFirstOrDefaultAsync(
                It.IsAny<Expression<Func<TextEntity, bool>>>(),
                null))
            .Throws(new Exception(DatabaseFailureMessage));

        Func<Task> act = () => _handler.Handle(command, CancellationToken.None);

        await act.Should()
            .ThrowAsync<Exception>()
            .WithMessage(DatabaseFailureMessage);

        _textRepoMock.Verify(
            r => r.GetFirstOrDefaultAsync(
                It.IsAny<Expression<Func<TextEntity, bool>>>(),
                null),
            Times.Once);

        _textRepoMock.Verify(
            r => r.Delete(It.IsAny<TextEntity>()),
            Times.Never);

        _repoWrapperMock.Verify(
            w => w.SaveChangesAsync(),
            Times.Never);

        _loggerMock.Verify(
            l => l.LogError(It.IsAny<object>(), It.IsAny<string>()),
            Times.Never);
    }
}
