using FluentValidation;
using Streetcode.BLL.MediatR.Streetcode.Text.GetParsed;
using Streetcode.BLL.Resources;

namespace Streetcode.BLL.Validators.Streetcode.Text.GetParsed
{
    public class GetParsedTextForAdminPreviewCommandValidator
        : AbstractValidator<GetParsedTextForAdminPreviewCommand>
    {
        public GetParsedTextForAdminPreviewCommandValidator()
        {
            RuleFor(x => x.textToParse)
                .NotEmpty()
                .WithMessage(ErrorMessages.TextToParseIsRequired);
        }
    }
}
