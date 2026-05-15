using FluentValidation;
using Streetcode.BLL.DTO.AdditionalContent.Tag;

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
            RuleFor(x => x.Title)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("Title is required")
                .Must(title => !string.IsNullOrWhiteSpace(title))
                .WithMessage("Title cannot contain only whitespace")
                .MaximumLength(100)
                .WithMessage("Title must not exceed 100 characters");
        }
    }
}
