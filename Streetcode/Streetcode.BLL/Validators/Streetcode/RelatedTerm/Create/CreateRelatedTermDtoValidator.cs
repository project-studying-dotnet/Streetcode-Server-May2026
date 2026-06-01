using FluentValidation;
using Streetcode.BLL.DTO.Streetcode.TextContent.RelatedTerm;
using Streetcode.BLL.Resources;

namespace Streetcode.BLL.Validators.Streetcode.RelatedTerm.Create
{
    public class CreateRelatedTermDtoValidator : AbstractValidator<CreateRelatedTermDTO>
    {
        public const int MaxWordLength = 50;
        public CreateRelatedTermDtoValidator()
        {
            RuleFor(dto => dto.Word)
                .NotEmpty().WithMessage(ErrorMessages.WordIsRequired)
                .MaximumLength(MaxWordLength).WithMessage(string.Format(
                    ErrorMessages.WordLengthError,
                    MaxWordLength));

            RuleFor(dto => dto.TermId)
                .GreaterThan(0).WithMessage(ErrorMessages.TermIdIsRequired);
        }
    }
}