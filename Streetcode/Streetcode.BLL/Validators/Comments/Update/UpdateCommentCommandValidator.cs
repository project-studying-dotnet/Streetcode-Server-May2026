using FluentValidation;
using Streetcode.BLL.MediatR.Comments.Update;
using Streetcode.BLL.Resources;

namespace Streetcode.BLL.Validators.Comments.Update;

public class UpdateCommentCommandValidator : AbstractValidator<UpdateCommentCommand>
{
    public UpdateCommentCommandValidator()
    {
        RuleLevelCascadeMode = CascadeMode.Stop;

        RuleFor(x => x.UpdateComment)
            .NotNull().WithMessage(ErrorMessages.CommentIsRequired)
            .SetValidator(new UpdateCommentDtoValidator());
    }
}
