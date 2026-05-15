using FluentValidation;
using Streetcode.BLL.DTO.AdditionalContent.Tag;
using Streetcode.BLL.MediatR.AdditionalContent.Tag.Create;

namespace Streetcode.BLL.Validators.AdditionalContent.Tag.Create
{
    /// <summary>
    /// Validator for CreateTagQuery.
    /// </summary>
    public class CreateTagQueryValidator : AbstractValidator<CreateTagQuery>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CreateTagQueryValidator"/> class.
        /// </summary>
        public CreateTagQueryValidator()
        {
            RuleFor(x => x.tag)
               .NotNull()
               .SetValidator(new CreateTagDtoValidator());
        }
    }
}
