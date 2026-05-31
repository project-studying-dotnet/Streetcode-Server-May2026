using FluentValidation;
using Streetcode.BLL.MediatR.Streetcode.RelatedFigure.Delete;
using Streetcode.BLL.Resources;

namespace Streetcode.BLL.Validators.Streetcode.RelatedFigure.Delete
{
    public class DeleteRelatedFigureCommandValidator : AbstractValidator<DeleteRelatedFigureCommand>
    {
        public DeleteRelatedFigureCommandValidator()
        {
            RuleFor(x => x.ObserverId)
                .GreaterThan(0)
                .WithMessage(ErrorMessages.ObserverIdMustBePositive);

            RuleFor(x => x.TargetId)
                .GreaterThan(0)
                .WithMessage(ErrorMessages.TargetIdMustBePositive);
        }
    }
}