using FluentValidation;
using Streetcode.BLL.DTO.AdditionalContent.Tag;
using Streetcode.BLL.Resources;

namespace Streetcode.BLL.Validators.AdditionalContent.Tag.Create
{
    public class CreateTagDtoValidator : AbstractValidator<CreateTagDTO>
    {
        public CreateTagDtoValidator()
        {
            RuleFor(x => x.Title)
                .Must(t => !string.IsNullOrWhiteSpace(t))
                .WithMessage(ErrorMessages.TitleIsRequired);
        }
    }
}
