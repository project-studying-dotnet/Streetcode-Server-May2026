using FluentValidation;
using Streetcode.BLL.DTO.AdditionalContent.Tag;
using Streetcode.BLL.Resources;

namespace Streetcode.BLL.Validators.AdditionalContent.Tag.Create
{
    /// <summary>
    /// Validator for CreateTagDTO.
    /// </summary>
    public class CreateTagDtoValidator : AbstractValidator<CreateTagDTO>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CreateTagDtoValidator"/> class.
        /// </summary>
        public CreateTagDtoValidator()
        {
            RuleLevelCascadeMode = CascadeMode.Stop;

            RuleFor(x => x.Title)
                .NotEmpty()
                .WithMessage(ErrorMessages.TitleIsRequired)
                .Must(title => title.Trim().Length > 0)
                .WithMessage(ErrorMessages.TitleCannotBeWhitespace);
        }
    }
}
