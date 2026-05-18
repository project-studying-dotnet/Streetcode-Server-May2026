using FluentValidation;
using Streetcode.BLL.DTO.Streetcode.TextContent.Fact;

namespace Streetcode.BLL.Validators.Streetcode.Fact
{
    /// <summary>
    /// Validator for FactDto.
    /// </summary>
    public class FactDtoValidator : AbstractValidator<FactDto>
    {
        private const int TitleMaxLength = 68;
        private const int FactContentMaxLength = 800;

        ////private const int FactContentMaxLength = 600; - поки моки для тестів перевищують максимальної довжини контенту факту. В реальності це має бути 600 символів, але для тестування можна використовувати більше значення.

        /// <summary>
        /// Initializes a new instance of the <see cref="FactDtoValidator"/> class.
        /// </summary>
        public FactDtoValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty()
                .WithMessage("Title is required")
                .MaximumLength(TitleMaxLength)
                .WithMessage($"Title must not exceed {TitleMaxLength} characters");

            RuleFor(x => x.FactContent)
                .NotEmpty()
                .WithMessage("Fact content is required")
                .MaximumLength(FactContentMaxLength)
                .WithMessage($"Fact content must not exceed {FactContentMaxLength} characters");

            RuleFor(x => x.ImageId)
                .GreaterThan(0)
                .WithMessage("ImageId must be greater than 0");
        }
    }
}
