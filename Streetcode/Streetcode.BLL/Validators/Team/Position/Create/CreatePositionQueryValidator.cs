using FluentValidation;
using Streetcode.BLL.MediatR.Team.Create;
using Streetcode.BLL.Resources;

namespace Streetcode.BLL.Validators.Team.Position.Create
{
    public class CreatePositionQueryValidator : AbstractValidator<CreatePositionQuery>
    {
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