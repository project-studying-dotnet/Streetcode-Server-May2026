using System;
using System.Linq.Expressions;
using FluentValidation.TestHelper;
using Moq;
using Streetcode.BLL.DTO.Sources;
using Streetcode.BLL.MediatR.Sources.StreetcodeCategoryContent.Update;
using Streetcode.BLL.Validators.Sources.StreetcodeCategoryContent.Update;
using Streetcode.DAL.Repositories.Interfaces.Base;
using Xunit;
using StreetcodeCategoryContentEntity = Streetcode.DAL.Entities.Sources.StreetcodeCategoryContent;

namespace Streetcode.XUnitTest.BLL.Validators.Sources.StreetcodeCategoryContent.Update;

public class UpdateStreetcodeCategoryContentCommandValidatorTests
{
    private const string CategoryContentText = "Some text";
    private readonly UpdateStreetcodeCategoryContentCommandValidator _validator;

    public UpdateStreetcodeCategoryContentCommandValidatorTests()
    {
        var repositoryWrapperMock = new Mock<IRepositoryWrapper>();

        repositoryWrapperMock
            .Setup(x => x.StreetcodeCategoryContentRepository.GetFirstOrDefaultAsync(
                It.IsAny<Expression<Func<StreetcodeCategoryContentEntity, bool>>?>(),
                null,
                default))
            .ReturnsAsync(new StreetcodeCategoryContentEntity());

        _validator = new UpdateStreetcodeCategoryContentCommandValidator(repositoryWrapperMock.Object);
    }

    [Fact]
    public async Task Should_Have_Error_When_CategoryContent_Is_Null()
    {
        var command = new UpdateStreetcodeCategoryContentCommand(null!);

        var result = await _validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(x => x.CategoryContent);
    }

    [Fact]
    public async Task Should_Have_Error_When_Text_Is_Empty()
    {
        var command = new UpdateStreetcodeCategoryContentCommand(new CategoryContentUpdateDto
        {
            Text = string.Empty,
            StreetcodeId = 1,
            SourceLinkCategoryId = 1
        });

        var result = await _validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(x => x.CategoryContent!.Text);
    }

    [Fact]
    public async Task Should_Have_Error_When_StreetcodeId_Is_Zero()
    {
        var command = new UpdateStreetcodeCategoryContentCommand(new CategoryContentUpdateDto
        {
            Text = CategoryContentText,
            StreetcodeId = 0,
            SourceLinkCategoryId = 1
        });

        var result = await _validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(x => x.CategoryContent!.StreetcodeId);
    }

    [Fact]
    public async Task Should_Have_Error_When_SourceLinkCategoryId_Is_Zero()
    {
        var command = new UpdateStreetcodeCategoryContentCommand(new CategoryContentUpdateDto
        {
            Text = CategoryContentText,
            StreetcodeId = 1,
            SourceLinkCategoryId = 0
        });

        var result = await _validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(x => x.CategoryContent!.SourceLinkCategoryId);
    }
}