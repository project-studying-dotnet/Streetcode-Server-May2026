using FluentValidation;
using Streetcode.BLL.MediatR.Media.Art.Create;
using Streetcode.BLL.Resources;
using Streetcode.BLL.Validators.Media.Art.Create;

namespace Streetcode.BLL.Validators.Media.Art
{
    public class CreateArtCommandValidator : AbstractValidator<CreateArtCommand>
    {
        public CreateArtCommandValidator()
        {
            RuleLevelCascadeMode = CascadeMode.Stop;

            RuleFor(command => command.ArtDto)
                .NotNull()
                .WithMessage(ErrorMessages.RequestIsRequired)
                .SetValidator(new ArtCreateDtoValidator());
        }
    }
}