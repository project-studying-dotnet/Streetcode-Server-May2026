using FluentValidation;
using Streetcode.BLL.DTO.Email;
using Streetcode.BLL.Resources;

namespace Streetcode.BLL.Validators.Email
{
    public class EmailDtoValidator : AbstractValidator<EmailDTO>
    {
        private const int MaxFromLength = 80;
        private const int MinContentLength = 1;
        private const int MaxContentLength = 500;
        public EmailDtoValidator()
        {
            RuleLevelCascadeMode = CascadeMode.Stop;

            RuleFor(x => x.From)
                .MaximumLength(MaxFromLength)
                .WithMessage(string.Format(ErrorMessages.EmailFromMustNotExceedCharacters, MaxFromLength));

            RuleFor(x => x.Content)
                .NotEmpty()
                .WithMessage(ErrorMessages.ContentIsRequired)
                .Length(MinContentLength, MaxContentLength)
                .WithMessage(string.Format(ErrorMessages.ContentLengthMustBeBetween, MinContentLength, MaxContentLength));
        }
    }
}
