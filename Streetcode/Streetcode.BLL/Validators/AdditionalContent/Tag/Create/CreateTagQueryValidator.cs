using FluentValidation;
using Streetcode.BLL.DTO.AdditionalContent.Tag;
using Streetcode.BLL.MediatR.AdditionalContent.Tag.Create;
using Streetcode.BLL.Resources;

namespace Streetcode.BLL.Validators.AdditionalContent.Tag.Create
{
    public class CreateTagQueryValidator : AbstractValidator<CreateTagQuery>
    {
        public CreateTagQueryValidator()
        {
            RuleFor(x => x.tag)
                .NotEmpty()
                .WithMessage(ErrorMessages.TagIsRequired)
                .SetValidator(new CreateTagDtoValidator());
        }
    }
}
