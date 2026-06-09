using FluentValidation;
using Streetcode.BLL.MediatR.Comments.Create;
using Streetcode.BLL.Resources;

namespace Streetcode.BLL.Validators.Comments.Create;

public class CreateCommentCommandValidator : AbstractValidator<CreateCommentCommand>
{
    public CreateCommentCommandValidator()
    {
        RuleLevelCascadeMode = CascadeMode.Stop;

        RuleFor(x => x.CreateComment)
            .NotNull().WithMessage(ErrorMessages.CommentIsRequired)
            .SetValidator(new CreateCommentDtoValidator());
    }
}
