using FluentValidation;
using Streetcode.BLL.MediatR.Streetcode.RelatedFigure.Create;
using Streetcode.DAL.Entities.AdditionalContent;

namespace Streetcode.BLL.Validators.Streetcode.RelatedFigure.Сreate
{
    /// <summary>
    /// Validator for CreateRelatedFigureCommand.
    /// </summary>
    public class CreateRelatedFigureCommandValidator : AbstractValidator<CreateRelatedFigureCommand>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CreateRelatedFigureCommandValidator"/> class.
        /// </summary>
        public CreateRelatedFigureCommandValidator()
        {
            RuleFor(x => x.ObserverId)
                .GreaterThan(0)
                .WithMessage("The ObserverId must be positive.");

            RuleFor(x => x.TargetId)
                .GreaterThan(0)
                .WithMessage("The TargetId must be positive.");
        }
    }
}