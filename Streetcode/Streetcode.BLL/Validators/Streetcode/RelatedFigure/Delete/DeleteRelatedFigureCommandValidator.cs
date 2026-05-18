using FluentValidation;
using Streetcode.BLL.MediatR.Streetcode.RelatedFigure.Delete;

namespace Streetcode.BLL.Validators.Streetcode.RelatedFigure.Delete
{
    /// <summary>
    /// Validator for DeleteRelatedFigureCommand.
    /// </summary>
    public class DeleteRelatedFigureCommandValidator : AbstractValidator<DeleteRelatedFigureCommand>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DeleteRelatedFigureCommandValidator"/> class.
        /// </summary>
        public DeleteRelatedFigureCommandValidator()
        {
            RuleFor(x => x.ObserverId)
                .GreaterThan(0);

            RuleFor(x => x.TargetId)
                .GreaterThan(0);
        }
    }
}