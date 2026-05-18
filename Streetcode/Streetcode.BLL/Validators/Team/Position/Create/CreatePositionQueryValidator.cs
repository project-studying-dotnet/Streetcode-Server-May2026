using FluentValidation;
using Streetcode.BLL.MediatR.Team.Create;

namespace Streetcode.BLL.Validators.Team.Position.Create
{
    /// <summary>
    /// Validator for <see cref="CreatePositionQuery"/>.
    /// </summary>
    public class CreatePositionQueryValidator : AbstractValidator<CreatePositionQuery>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CreatePositionQueryValidator"/> class.
        /// </summary>
        public CreatePositionQueryValidator()
        {
            RuleFor(x => x.position)
                .NotNull()
                .WithMessage("Position is required")
                .SetValidator(new PositionDtoValidator());
        }
    }
}