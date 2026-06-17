using FluentValidation;
using Streetcode.BLL.MediatR.Comments.Delete;
using Streetcode.BLL.Resources;

namespace Streetcode.BLL.Validators.Comments.Delete;

public class DeleteCommentCommandValidator : AbstractValidator<DeleteCommentCommand>
{
    public DeleteCommentCommandValidator()
    {
        RuleFor(x => x.CommentId)
            .GreaterThan(0).WithMessage(ErrorMessages.IdMustBePositive);
    }
}
