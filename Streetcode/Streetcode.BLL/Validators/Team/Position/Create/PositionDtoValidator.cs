using FluentValidation;
using Streetcode.BLL.DTO.Team;
using Streetcode.BLL.Resources;

namespace Streetcode.BLL.Validators.Team.Position.Create
{
    /// <summary>
    /// Validator for PositionDTO.
    /// </summary>
    public class PositionDtoValidator : AbstractValidator<PositionDTO>
    {
        public PositionDtoValidator()
        {
            RuleLevelCascadeMode = CascadeMode.Stop;

            RuleFor(x => x.Position)
                .NotEmpty()
                .WithMessage(ErrorMessages.PositionNameIsRequired);
        }
    }
}