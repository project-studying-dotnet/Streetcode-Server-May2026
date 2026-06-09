using FluentValidation;
using Streetcode.BLL.DTO.Comments;
using Streetcode.BLL.Resources;

namespace Streetcode.BLL.Validators.Comments.Create;

public class CreateCommentDtoValidator : AbstractValidator<CreateCommentDto>
{
    public const int MaxTextLength = 1000;

    public CreateCommentDtoValidator()
    {
        RuleLevelCascadeMode = CascadeMode.Stop;

        RuleFor(x => x.Text)
            .NotEmpty().WithMessage(ErrorMessages.TextIsRequired)
            .MaximumLength(MaxTextLength).WithMessage(string.Format(
                ErrorMessages.TextMustNotExceedCharacters,
                MaxTextLength));

        RuleFor(x => x.StreetcodeId)
            .GreaterThan(0).WithMessage(ErrorMessages.StreetcodeIdMustBePositive);

        RuleFor(x => x.UserId)
            .GreaterThan(0).WithMessage(ErrorMessages.IdMustBePositive)
            .When(x => x.UserId.HasValue);

        RuleFor(x => x.ParentCommentId)
            .GreaterThan(0).WithMessage(ErrorMessages.IdMustBePositive)
            .When(x => x.ParentCommentId.HasValue);
    }
}
