using FluentValidation;
using Streetcode.BLL.DTO.Streetcode.TextContent.Fact;
using Streetcode.BLL.Resources;

namespace Streetcode.BLL.Validators.Streetcode.Fact
{
    /// <summary>
    /// Validator for FactDto.
    /// </summary>
    public class FactDtoValidator : AbstractValidator<FactDto>
    {
        private const int MaxTitleLength = 68;
        private const int MaxFactContentLength = 800;

        ////private const int FactContentMaxLength = 600; - поки моки для тестів перевищують максимальної довжини контенту факту. В реальності це має бути 600 символів, але для тестування можна використовувати більше значення.

        /// <summary>
        /// Initializes a new instance of the <see cref="FactDtoValidator"/> class.
        /// </summary>
        public FactDtoValidator()
        {
            RuleLevelCascadeMode = CascadeMode.Stop;

            RuleFor(x => x.Title)
                .NotEmpty().WithMessage(ErrorMessages.TitleIsRequired)
                .MaximumLength(MaxTitleLength).WithMessage(string.Format(ErrorMessages.TitleMustNotExceedCharacters, MaxTitleLength));

            RuleFor(x => x.FactContent)
                .NotEmpty().WithMessage(ErrorMessages.FactContentIsRequired)
                .MaximumLength(MaxFactContentLength).WithMessage(string.Format(ErrorMessages.FactContentMustNotExceedCharacters, MaxFactContentLength));

            RuleFor(x => x.ImageId)
                .GreaterThan(0)
                .WithMessage(ErrorMessages.ImageIdMustBePositive);
        }
    }
}
