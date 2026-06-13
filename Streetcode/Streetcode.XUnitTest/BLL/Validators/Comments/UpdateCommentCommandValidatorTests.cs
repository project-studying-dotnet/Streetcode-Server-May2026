using FluentValidation.TestHelper;
using Streetcode.BLL.DTO.Comments;
using Streetcode.BLL.MediatR.Comments.Update;
using Streetcode.BLL.Resources;
using Streetcode.BLL.Validators.Comments.Update;
using Xunit;

namespace Streetcode.XUnitTest.BLL.Validators.Comments;

public class UpdateCommentCommandValidatorTests
{
    private readonly UpdateCommentCommandValidator _validator;

    public UpdateCommentCommandValidatorTests()
    {
        _validator = new UpdateCommentCommandValidator();
    }

    [Fact]
    public void Should_Have_Error_When_UpdateComment_Is_Null()
    {
        var command = new UpdateCommentCommand(null!);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.UpdateComment)
            .WithErrorMessage(ErrorMessages.CommentIsRequired);
    }

    [Fact]
    public void Should_Not_Have_Errors_When_Command_Is_Fully_Valid()
    {
        var command = new UpdateCommentCommand(new UpdateCommentDto
        {
            Id = 1,
            Text = "Valid updated comment",
            StreetcodeId = 1,
        });

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveAnyValidationErrors();
    }
}
