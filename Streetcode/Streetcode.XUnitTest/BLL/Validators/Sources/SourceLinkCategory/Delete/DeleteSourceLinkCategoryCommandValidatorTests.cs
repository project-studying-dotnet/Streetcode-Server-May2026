using System;
using System.Linq.Expressions;
using FluentValidation.TestHelper;
using Moq;
using Streetcode.BLL.MediatR.Sources.SourceLinkCategory.Delete;
using Streetcode.BLL.Validators.Sources.SourceLinkCategory.Delete;
using Streetcode.DAL.Repositories.Interfaces.Base;
using Xunit;
using SourceLinkCategoryEntity = Streetcode.DAL.Entities.Sources.SourceLinkCategory;

namespace Streetcode.XUnitTest.BLL.Validators.Sources.SourceLinkCategory.Delete;

public class DeleteSourceLinkCategoryCommandValidatorTests
{
    private readonly DeleteSourceLinkCategoryCommandValidator _validator;

    public DeleteSourceLinkCategoryCommandValidatorTests()
    {
        var repositoryWrapperMock = new Mock<IRepositoryWrapper>();

        repositoryWrapperMock
            .Setup(x => x.SourceCategoryRepository.GetFirstOrDefaultAsync(
                It.IsAny<Expression<Func<SourceLinkCategoryEntity, bool>>>(),
                null))
            .ReturnsAsync(new SourceLinkCategoryEntity());

        _validator = new DeleteSourceLinkCategoryCommandValidator(repositoryWrapperMock.Object);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public async Task Should_Have_Error_When_Id_Is_Invalid(int id)
    {
        var command = new DeleteSourceLinkCategoryCommand(id);

        var result = await _validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(x => x.Id);
    }

    [Fact]
    public async Task Should_Not_Have_Errors_When_Command_Is_Valid()
    {
        var command = new DeleteSourceLinkCategoryCommand(1);

        var result = await _validator.TestValidateAsync(command);

        result.ShouldNotHaveAnyValidationErrors();
    }
}