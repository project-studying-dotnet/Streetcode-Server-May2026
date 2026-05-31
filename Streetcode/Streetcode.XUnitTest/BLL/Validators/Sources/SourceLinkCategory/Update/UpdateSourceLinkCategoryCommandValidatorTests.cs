using System;
using System.Linq.Expressions;
using FluentValidation.TestHelper;
using Moq;
using Streetcode.BLL.DTO.Sources;
using Streetcode.BLL.MediatR.Sources.SourceLinkCategory.Update;
using Streetcode.BLL.Validators.Sources.SourceLinkCategory.Update;
using Streetcode.DAL.Repositories.Interfaces.Base;
using Xunit;
using SourceLinkCategoryEntity = Streetcode.DAL.Entities.Sources.SourceLinkCategory;

namespace Streetcode.XUnitTest.BLL.Validators.Sources.SourceLinkCategory.Update;

public class UpdateSourceLinkCategoryCommandValidatorTests
{
    private readonly UpdateSourceLinkCategoryCommandValidator _validator;

    public UpdateSourceLinkCategoryCommandValidatorTests()
    {
        var repositoryWrapperMock = new Mock<IRepositoryWrapper>();

        repositoryWrapperMock
            .SetupSequence(x => x.SourceCategoryRepository.GetFirstOrDefaultAsync(
                It.IsAny<Expression<Func<SourceLinkCategoryEntity, bool>>?>(),
                null,
                default))
            .ReturnsAsync(new SourceLinkCategoryEntity())
            .ReturnsAsync((SourceLinkCategoryEntity?)null);

        _validator = new UpdateSourceLinkCategoryCommandValidator(repositoryWrapperMock.Object);
    }

    [Fact]
    public async Task Should_Have_Error_When_Category_Is_Null()
    {
        var command = new UpdateSourceLinkCategoryCommand(null!);

        var result = await _validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(x => x.Category);
    }

    [Fact]
    public async Task Should_Have_Error_When_Id_Is_Zero()
    {
        var command = new UpdateSourceLinkCategoryCommand(new SourceLinkCategoryDTO
        {
            Id = 0,
            Title = "Books",
            ImageId = 1
        });

        var result = await _validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(x => x.Category!.Id);
    }

    [Fact]
    public async Task Should_Have_Error_When_Title_Is_Empty()
    {
        var command = new UpdateSourceLinkCategoryCommand(new SourceLinkCategoryDTO
        {
            Id = 1,
            Title = string.Empty,
            ImageId = 1
        });

        var result = await _validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(x => x.Category!.Title);
    }

    [Fact]
    public async Task Should_Have_Error_When_ImageId_Is_Zero()
    {
        var command = new UpdateSourceLinkCategoryCommand(new SourceLinkCategoryDTO
        {
            Id = 1,
            Title = "Books",
            ImageId = 0
        });

        var result = await _validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(x => x.Category!.ImageId);
    }

    [Fact]
    public async Task Should_Not_Have_Errors_When_Command_Is_Valid()
    {
        var command = new UpdateSourceLinkCategoryCommand(new SourceLinkCategoryDTO
        {
            Id = 1,
            Title = "Books",
            ImageId = 1
        });

        var result = await _validator.TestValidateAsync(command);

        result.ShouldNotHaveAnyValidationErrors();
    }
}