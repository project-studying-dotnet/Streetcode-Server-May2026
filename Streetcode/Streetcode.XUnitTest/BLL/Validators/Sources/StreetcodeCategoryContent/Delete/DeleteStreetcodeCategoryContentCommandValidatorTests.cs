using System;
using System.Linq.Expressions;
using FluentValidation.TestHelper;
using Moq;
using Streetcode.BLL.MediatR.Sources.StreetcodeCategoryContent.Delete;
using Streetcode.BLL.Validators.Sources.StreetcodeCategoryContent.Delete;
using Streetcode.DAL.Repositories.Interfaces.Base;
using Xunit;
using StreetcodeCategoryContentEntity = Streetcode.DAL.Entities.Sources.StreetcodeCategoryContent;

namespace Streetcode.XUnitTest.BLL.Validators.Sources.StreetcodeCategoryContent.Delete;

public class DeleteStreetcodeCategoryContentCommandValidatorTests
{
    private readonly DeleteStreetcodeCategoryContentCommandValidator _validator;

    public DeleteStreetcodeCategoryContentCommandValidatorTests()
    {
        var repositoryWrapperMock = new Mock<IRepositoryWrapper>();

        repositoryWrapperMock
            .Setup(x => x.StreetcodeCategoryContentRepository.GetFirstOrDefaultAsync(
                It.IsAny<Expression<Func<StreetcodeCategoryContentEntity, bool>>?>(),
                null,
                default))
            .ReturnsAsync(new StreetcodeCategoryContentEntity());

        _validator = new DeleteStreetcodeCategoryContentCommandValidator(repositoryWrapperMock.Object);
    }

    [Fact]
    public async Task Should_Have_Error_When_StreetcodeId_Is_Zero()
    {
        var command = new DeleteStreetcodeCategoryContentCommand(0, 1);

        var result = await _validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(x => x.StreetcodeId);
    }

    [Fact]
    public async Task Should_Have_Error_When_SourceLinkCategoryId_Is_Zero()
    {
        var command = new DeleteStreetcodeCategoryContentCommand(1, 0);

        var result = await _validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(x => x.SourceLinkCategoryId);
    }

    [Fact]
    public async Task Should_Not_Have_Errors_When_Command_Is_Valid()
    {
        var command = new DeleteStreetcodeCategoryContentCommand(1, 1);

        var result = await _validator.TestValidateAsync(command);

        result.ShouldNotHaveAnyValidationErrors();
    }
}