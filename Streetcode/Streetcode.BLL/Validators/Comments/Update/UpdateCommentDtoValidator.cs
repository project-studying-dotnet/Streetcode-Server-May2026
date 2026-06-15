using FluentValidation;
using Streetcode.BLL.DTO.Comments;
using Streetcode.BLL.Resources;
using Streetcode.BLL.Validators.Comments.Create;

namespace Streetcode.BLL.Validators.Comments.Update;

public class UpdateCommentDtoValidator : AbstractValidator<UpdateCommentDto>
{
    public UpdateCommentDtoValidator()
    {
        RuleLevelCascadeMode = CascadeMode.Stop;

        RuleFor(x => x.Id)
            .GreaterThan(0).WithMessage(ErrorMessages.IdMustBePositive);

        RuleFor(x => x.Text)
            .NotEmpty().WithMessage(ErrorMessages.TextIsRequired)
            .MaximumLength(CreateCommentDtoValidator.MaxTextLength).WithMessage(string.Format(
                ErrorMessages.TextMustNotExceedCharacters,
                CreateCommentDtoValidator.MaxTextLength));

        RuleFor(x => x.StreetcodeId)
            .GreaterThan(0).WithMessage(ErrorMessages.StreetcodeIdMustBePositive);

        RuleFor(x => x.UserId)
            .GreaterThan(0).WithMessage(ErrorMessages.IdMustBePositive)
            .When(x => x.UserId.HasValue);

        RuleFor(x => x.ParentCommentId)
            .GreaterThan(0).WithMessage(ErrorMessages.IdMustBePositive)
            .When(x => x.ParentCommentId.HasValue);

        RuleFor(x => x.ParentCommentId)
            .NotEqual(x => x.Id).WithMessage(ErrorMessages.CommentCannotBeItsOwnParent)
            .When(x => x.ParentCommentId.HasValue);
    }
}
