using FluentValidation;
using Streetcode.BLL.DTO.AdditionalContent.Tag;
using Streetcode.BLL.MediatR.AdditionalContent.Tag.Create;
using Streetcode.BLL.Resources;

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
            RuleLevelCascadeMode = CascadeMode.Stop;

            RuleFor(x => x.tag)
                .NotNull()
                .WithMessage(ErrorMessages.TagIsRequired)
                .SetValidator(new CreateTagDtoValidator());
        }
    }
}
