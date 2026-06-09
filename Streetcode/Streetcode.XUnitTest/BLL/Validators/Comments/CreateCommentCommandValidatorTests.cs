using FluentValidation.TestHelper;
using Streetcode.BLL.DTO.Comments;
using Streetcode.BLL.MediatR.Comments.Create;
using Streetcode.BLL.Resources;
using Streetcode.BLL.Validators.Comments.Create;
using Xunit;

namespace Streetcode.XUnitTest.BLL.Validators.Comments;

public class CreateCommentCommandValidatorTests
{
    private readonly CreateCommentCommandValidator _validator;

    public CreateCommentCommandValidatorTests()
    {
        _validator = new CreateCommentCommandValidator();
    }

    [Fact]
    public void Should_Have_Error_When_CreateComment_Is_Null()
    {
        var command = new CreateCommentCommand(null!);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.CreateComment)
            .WithErrorMessage(ErrorMessages.CommentIsRequired);
    }

    [Fact]
    public void Should_Not_Have_Errors_When_Command_Is_Fully_Valid()
    {
        var command = new CreateCommentCommand(new CreateCommentDto
        {
            Text = "Valid comment",
            StreetcodeId = 1,
        });

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveAnyValidationErrors();
    }
}
