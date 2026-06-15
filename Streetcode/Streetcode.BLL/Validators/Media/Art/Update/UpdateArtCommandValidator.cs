using FluentValidation;
using Streetcode.BLL.MediatR.Media.Art.Update;
using Streetcode.BLL.Resources;

namespace Streetcode.BLL.Validators.Media.Art
{
    public class UpdateArtCommandValidator : AbstractValidator<UpdateArtCommand>
    {
        public UpdateArtCommandValidator()
        {
            RuleLevelCascadeMode = CascadeMode.Stop;

            RuleFor(command => command.ArtDto)
                .NotNull()
                .WithMessage(ErrorMessages.RequestIsRequired)
                .SetValidator(new ArtUpdateDtoValidator());
        }
    }
}