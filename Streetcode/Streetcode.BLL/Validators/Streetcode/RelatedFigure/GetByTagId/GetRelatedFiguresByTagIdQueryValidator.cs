using FluentValidation;
using Streetcode.BLL.MediatR.Streetcode.RelatedFigure.GetByTagId;
using Streetcode.BLL.Resources;

namespace Streetcode.BLL.Validators.Streetcode.RelatedFigure.GetByTagId
{
    public class GetRelatedFiguresByTagIdQueryValidator
       : AbstractValidator<GetRelatedFiguresByTagIdQuery>
    {
        public GetRelatedFiguresByTagIdQueryValidator()
        {
            RuleFor(x => x.tagId)
                    .GreaterThan(0)
                    .WithMessage(ErrorMessages.TagIdMustBePositive);
        }
    }
}
