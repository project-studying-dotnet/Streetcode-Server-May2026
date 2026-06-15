using FluentValidation.TestHelper;
using Streetcode.BLL.DTO.Comments;
using Streetcode.BLL.Resources;
using Streetcode.BLL.Validators.Comments.Create;
using Streetcode.BLL.Validators.Comments.Update;
using Xunit;

namespace Streetcode.XUnitTest.BLL.Validators.Comments;

public class UpdateCommentDtoValidatorTests
{
    private const int MaxTextLength = CreateCommentDtoValidator.MaxTextLength;
    private readonly UpdateCommentDtoValidator _validator;

    public UpdateCommentDtoValidatorTests()
    {
        _validator = new UpdateCommentDtoValidator();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Should_Have_Error_When_Id_Is_Less_Or_Equal_To_Zero(int invalidId)
    {
        var dto = CreateValidDto();
        dto.Id = invalidId;

        var result = _validator.TestValidate(dto);

        result.ShouldHaveValidationErrorFor(x => x.Id)
            .WithErrorMessage(ErrorMessages.IdMustBePositive);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Should_Have_Error_When_Text_Is_Empty(string? invalidText)
    {
        var dto = CreateValidDto();
        dto.Text = invalidText!;

        var result = _validator.TestValidate(dto);

        result.ShouldHaveValidationErrorFor(x => x.Text)
            .WithErrorMessage(ErrorMessages.TextIsRequired);
    }

    [Fact]
    public void Should_Have_Error_When_Text_Exceeds_Maximum_Length()
    {
        var dto = CreateValidDto();
        dto.Text = new string('A', MaxTextLength + 1);

        var result = _validator.TestValidate(dto);

        result.ShouldHaveValidationErrorFor(x => x.Text)
            .WithErrorMessage(string.Format(ErrorMessages.TextMustNotExceedCharacters, MaxTextLength));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Should_Have_Error_When_StreetcodeId_Is_Less_Or_Equal_To_Zero(int invalidStreetcodeId)
    {
        var dto = CreateValidDto();
        dto.StreetcodeId = invalidStreetcodeId;

        var result = _validator.TestValidate(dto);

        result.ShouldHaveValidationErrorFor(x => x.StreetcodeId)
            .WithErrorMessage(ErrorMessages.StreetcodeIdMustBePositive);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Should_Have_Error_When_UserId_Is_Less_Or_Equal_To_Zero(int invalidUserId)
    {
        var dto = CreateValidDto();
        dto.UserId = invalidUserId;

        var result = _validator.TestValidate(dto);

        result.ShouldHaveValidationErrorFor(x => x.UserId)
            .WithErrorMessage(ErrorMessages.IdMustBePositive);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Should_Have_Error_When_ParentCommentId_Is_Less_Or_Equal_To_Zero(int invalidParentCommentId)
    {
        var dto = CreateValidDto();
        dto.ParentCommentId = invalidParentCommentId;

        var result = _validator.TestValidate(dto);

        result.ShouldHaveValidationErrorFor(x => x.ParentCommentId)
            .WithErrorMessage(ErrorMessages.IdMustBePositive);
    }

    [Fact]
    public void Should_Have_Error_When_ParentCommentId_Equals_Id()
    {
        var dto = CreateValidDto();
        dto.ParentCommentId = dto.Id;

        var result = _validator.TestValidate(dto);

        result.ShouldHaveValidationErrorFor(x => x.ParentCommentId)
            .WithErrorMessage(ErrorMessages.CommentCannotBeItsOwnParent);
    }

    [Fact]
    public void Should_Not_Have_Errors_When_Dto_Is_Valid()
    {
        var dto = CreateValidDto();

        var result = _validator.TestValidate(dto);

        result.ShouldNotHaveAnyValidationErrors();
    }

    private static UpdateCommentDto CreateValidDto()
    {
        return new UpdateCommentDto
        {
            Id = 1,
            Text = "Valid updated comment",
            StreetcodeId = 1,
            UserId = 10,
            ParentCommentId = 2,
        };
    }
}
