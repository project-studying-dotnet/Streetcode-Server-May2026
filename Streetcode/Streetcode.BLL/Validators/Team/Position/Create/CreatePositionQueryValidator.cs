using FluentValidation;
using Streetcode.BLL.MediatR.Team.Create;
using Streetcode.BLL.Resources;

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
            RuleLevelCascadeMode = CascadeMode.Stop;

            RuleFor(x => x.position)
                .NotNull()
                .WithMessage(ErrorMessages.PositionIsRequired)
                .SetValidator(new PositionDtoValidator());
        }
    }
}