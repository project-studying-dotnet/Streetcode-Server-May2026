using FluentValidation;
using Streetcode.BLL.MediatR.Streetcode.Text.GetParsed;
using Streetcode.BLL.Resources;

namespace Streetcode.BLL.Validators.Streetcode.Text.GetParsed
{
    /// <summary>
    /// Validator for <see cref="GetParsedTextForAdminPreviewCommand"/>.
    /// </summary>
    public class GetParsedTextForAdminPreviewCommandValidator
        : AbstractValidator<GetParsedTextForAdminPreviewCommand>
    {
        //private const int MaxTextLength = 10000;

        /// <summary>
        /// Initializes a new instance of the <see cref="GetParsedTextForAdminPreviewCommandValidator"/> class.
        /// </summary>
        public GetParsedTextForAdminPreviewCommandValidator()
        {
            RuleFor(x => x.textToParse)
                .NotEmpty()
                .WithMessage(ErrorMessages.TextToParseIsRequired);
                //.MaximumLength(MaxTextLength)
                //.WithMessage(string.Format(ErrorMessages.TextToParseMustNotExceedCharacters, MaxTextLength));
        }
    }
}
