using FluentValidation;
using Streetcode.BLL.DTO.Media.Art;
using Streetcode.BLL.Resources;

namespace Streetcode.BLL.Validators.Media.Art
{
    public class ArtUpdateDtoValidator : AbstractValidator<ArtUpdateDto>
    {
        public const int MaxTitleLength = 150;
        public const int MaxDescriptionLength = 1000;

        public ArtUpdateDtoValidator()
        {
            RuleLevelCascadeMode = CascadeMode.Stop;

            RuleFor(dto => dto.Id)
                .GreaterThan(0)
                .WithMessage(ErrorMessages.InvalidId);

            RuleFor(dto => dto.ImageId)
                .GreaterThan(0)
                .WithMessage(ErrorMessages.InvalidId);

            RuleFor(dto => dto.Title)
                .NotEmpty()
                .WithMessage(ErrorMessages.TitleIsRequired)
                .MaximumLength(MaxTitleLength)
                .WithMessage(string.Format(ErrorMessages.MaxLengthError, "Title", MaxTitleLength));

            RuleFor(dto => dto.Description)
                .MaximumLength(MaxDescriptionLength)
                .WithMessage(string.Format(ErrorMessages.MaxLengthError, "Description", MaxDescriptionLength));
        }
    }
}