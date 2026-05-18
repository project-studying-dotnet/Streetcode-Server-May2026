using FluentValidation;
using Streetcode.BLL.DTO.Team;

namespace Streetcode.BLL.Validators.Team.Position.Create
{
    /// <summary>
    /// Validator for PositionDTO.
    /// </summary>
    public class PositionDtoValidator : AbstractValidator<PositionDTO>
    {
        public PositionDtoValidator()
        {
            RuleFor(x => x.Position)
                .NotEmpty()
                .WithMessage("Position name is required");
                ////.MaximumLength(100)
                ////.WithMessage("Position name must not exceed 100 characters");
        }
    }
}