using FluentValidation;
using Streetcode.BLL.MediatR.Comments.GetRepliesByCommentId;
using Streetcode.BLL.Resources;

namespace Streetcode.BLL.Validators.Comments.GetRepliesByCommentId;

public class GetRepliesByCommentIdQueryValidator : AbstractValidator<GetRepliesByCommentIdQuery>
{
    public GetRepliesByCommentIdQueryValidator()
    {
        RuleFor(x => x.CommentId)
            .GreaterThan(0).WithMessage(ErrorMessages.IdMustBePositive);
    }
}
