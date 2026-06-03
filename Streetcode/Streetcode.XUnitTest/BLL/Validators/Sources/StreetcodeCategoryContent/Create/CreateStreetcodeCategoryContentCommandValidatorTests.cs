using System;
using System.Linq.Expressions;
using FluentValidation.TestHelper;
using Moq;
using Streetcode.BLL.DTO.Sources;
using Streetcode.BLL.MediatR.Sources.StreetcodeCategoryContent.Create;
using Streetcode.BLL.Validators.Sources.StreetcodeCategoryContent.Create;
using Streetcode.DAL.Entities.Streetcode;
using Streetcode.DAL.Repositories.Interfaces.Base;
using Xunit;
using SourceLinkCategoryEntity = Streetcode.DAL.Entities.Sources.SourceLinkCategory;
using StreetcodeCategoryContentEntity = Streetcode.DAL.Entities.Sources.StreetcodeCategoryContent;

namespace Streetcode.XUnitTest.BLL.Validators.Sources.StreetcodeCategoryContent.Create;

public class CreateStreetcodeCategoryContentCommandValidatorTests
{
    private const string CategoryText = "Some text";

    private readonly CreateStreetcodeCategoryContentCommandValidator _validator;

    public CreateStreetcodeCategoryContentCommandValidatorTests()
    {
        var repositoryWrapperMock = new Mock<IRepositoryWrapper>();

        repositoryWrapperMock
            .Setup(x => x.StreetcodeRepository.GetFirstOrDefaultAsync(
                It.IsAny<Expression<Func<StreetcodeContent, bool>>?>(),
                null,
                default))
            .ReturnsAsync(new StreetcodeContent());

        repositoryWrapperMock
            .Setup(x => x.SourceCategoryRepository.GetFirstOrDefaultAsync(
                It.IsAny<Expression<Func<SourceLinkCategoryEntity, bool>>?>(),
                null,
                default))
            .ReturnsAsync(new SourceLinkCategoryEntity());

        repositoryWrapperMock
            .Setup(x => x.StreetcodeCategoryContentRepository.GetFirstOrDefaultAsync(
                It.IsAny<Expression<Func<StreetcodeCategoryContentEntity, bool>>?>(),
                null,
                default))
            .ReturnsAsync((StreetcodeCategoryContentEntity?)null);

        _validator = new CreateStreetcodeCategoryContentCommandValidator(repositoryWrapperMock.Object);
    }

    [Fact]
    public async Task Should_Have_Error_When_CategoryContent_Is_Null()
    {
        var command = new CreateStreetcodeCategoryContentCommand(null!);

        var result = await _validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(x => x.CategoryContent);
    }

    [Fact]
    public async Task Should_Have_Error_When_Text_Is_Empty()
    {
        var command = new CreateStreetcodeCategoryContentCommand(new CategoryContentCreateDto
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
        var command = new CreateStreetcodeCategoryContentCommand(new CategoryContentCreateDto
        {
            Text = CategoryText,
            StreetcodeId = 0,
            SourceLinkCategoryId = 1
        });

        var result = await _validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(x => x.CategoryContent!.StreetcodeId);
    }

    [Fact]
    public async Task Should_Not_Have_Errors_When_Command_Is_Valid()
    {
        var command = new CreateStreetcodeCategoryContentCommand(new CategoryContentCreateDto
        {
            Text = CategoryText,
            StreetcodeId = 1,
            SourceLinkCategoryId = 1
        });

        var result = await _validator.TestValidateAsync(command);

        result.ShouldNotHaveAnyValidationErrors();
    }
}