using FluentValidation;
using Streetcode.BLL.MediatR.Comments.GetByStreetcodeId;
using Streetcode.BLL.Resources;

namespace Streetcode.BLL.Validators.Comments.GetByStreetcodeId;

public class GetCommentsByStreetcodeIdQueryValidator : AbstractValidator<GetCommentsByStreetcodeIdQuery>
{
    public GetCommentsByStreetcodeIdQueryValidator()
    {
        RuleFor(x => x.StreetcodeId)
            .GreaterThan(0).WithMessage(ErrorMessages.StreetcodeIdMustBePositive);
    }
}
