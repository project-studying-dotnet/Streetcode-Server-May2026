using FluentValidation;
using Streetcode.BLL.MediatR.Streetcode.Text.GetParsed;

namespace Streetcode.BLL.Validators.Streetcode.Text.GetParsed
{
    /// <summary>
    /// Validator for <see cref="GetParsedTextForAdminPreviewCommand"/>.
    /// </summary>
    public class GetParsedTextForAdminPreviewCommandValidator
        : AbstractValidator<GetParsedTextForAdminPreviewCommand>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GetParsedTextForAdminPreviewCommandValidator"/> class.
        /// </summary>
        public GetParsedTextForAdminPreviewCommandValidator()
        {
            RuleFor(x => x.textToParse)
                .NotEmpty()
                .WithMessage("Text to parse is required");
                ////.MaximumLength(10000)
                ////.WithMessage("Text to parse must not exceed 10000 characters");
        }
    }
}
