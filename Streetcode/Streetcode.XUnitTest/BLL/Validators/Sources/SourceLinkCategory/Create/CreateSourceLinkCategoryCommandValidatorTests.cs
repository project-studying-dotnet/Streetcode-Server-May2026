using System;
using System.Linq.Expressions;
using FluentValidation.TestHelper;
using Moq;
using Streetcode.BLL.DTO.Sources;
using Streetcode.BLL.MediatR.Sources.SourceLinkCategory.Create;
using Streetcode.BLL.Validators.Sources.SourceLinkCategory.Create;
using Streetcode.DAL.Repositories.Interfaces.Base;
using Xunit;
using SourceLinkCategoryEntity = Streetcode.DAL.Entities.Sources.SourceLinkCategory;

namespace Streetcode.XUnitTest.BLL.Validators.Sources.SourceLinkCategory.Create;

public class CreateSourceLinkCategoryCommandValidatorTests
{
    private const string TitleName = "Books";
    private readonly CreateSourceLinkCategoryCommandValidator _validator;

    public CreateSourceLinkCategoryCommandValidatorTests()
    {
        var repositoryWrapperMock = new Mock<IRepositoryWrapper>();

        repositoryWrapperMock
            .Setup(x => x.SourceCategoryRepository.GetFirstOrDefaultAsync(
                It.IsAny<Expression<Func<SourceLinkCategoryEntity, bool>>?>(),
                null,
                default))
            .ReturnsAsync((SourceLinkCategoryEntity?)null);

        _validator = new CreateSourceLinkCategoryCommandValidator(repositoryWrapperMock.Object);
    }

    [Fact]
    public async Task Should_Have_Error_When_Category_Is_Null()
    {
        var command = new CreateSourceLinkCategoryCommand(null!);

        var result = await _validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(x => x.Category);
    }

    [Fact]
    public async Task Should_Have_Error_When_Title_Is_Empty()
    {
        var command = new CreateSourceLinkCategoryCommand(new SourceLinkCategoryDto
        {
            Title = string.Empty,
            ImageId = 1
        });

        var result = await _validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(x => x.Category!.Title);
    }

    [Fact]
    public async Task Should_Have_Error_When_ImageId_Is_Zero()
    {
        var command = new CreateSourceLinkCategoryCommand(new SourceLinkCategoryDto
        {
            Title = TitleName,
            ImageId = 0
        });

        var result = await _validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(x => x.Category!.ImageId);
    }

    [Fact]
    public async Task Should_Not_Have_Errors_When_Command_Is_Valid()
    {
        var command = new CreateSourceLinkCategoryCommand(new SourceLinkCategoryDto
        {
            Title = TitleName,
            ImageId = 1
        });

        var result = await _validator.TestValidateAsync(command);

        result.ShouldNotHaveAnyValidationErrors();
    }
}