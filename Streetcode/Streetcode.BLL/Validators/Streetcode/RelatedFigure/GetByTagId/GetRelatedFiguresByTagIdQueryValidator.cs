using FluentValidation;
using Streetcode.BLL.MediatR.Streetcode.RelatedFigure.GetByTagId;

namespace Streetcode.BLL.Validators.Streetcode.RelatedFigure.GetByTagId
{
    /// <summary>
    /// Validator for GetRelatedFiguresByTagIdQuery.
    /// </summary>
    public class GetRelatedFiguresByTagIdQueryValidator
       : AbstractValidator<GetRelatedFiguresByTagIdQuery>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GetRelatedFiguresByTagIdQueryValidator"/> class.
        /// </summary>
        public GetRelatedFiguresByTagIdQueryValidator()
        {
            RuleFor(x => x.tagId)
                    .GreaterThan(0)
                    .WithMessage("The tagId must be positive.");
        }
    }
}
