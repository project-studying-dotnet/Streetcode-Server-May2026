using FluentValidation;
using Streetcode.BLL.MediatR.Newss.GetNewsAndLinksByUrl;
using Streetcode.BLL.Resources;

namespace Streetcode.BLL.Validators.Newss.GetNewsAndLinksByUrl
{
    /// <summary>
    /// Validator for GetNewsAndLinksByUrlQuery.
    /// </summary>
    public class GetNewsAndLinksByUrlValidator : AbstractValidator<GetNewsAndLinksByUrlQuery>
    {
        private const int MaxUrlLength = 2048;

        /// <summary>
        /// Initializes a new instance of the <see cref="GetNewsAndLinksByUrlValidator"/> class.
        /// </summary>
        public GetNewsAndLinksByUrlValidator()
        {
            RuleLevelCascadeMode = CascadeMode.Stop;

            RuleFor(x => x.url)
                .NotEmpty()
                .WithMessage(ErrorMessages.UrlIsRequired)
                .MaximumLength(MaxUrlLength)
                .WithMessage(string.Format(ErrorMessages.UrlMustNotExceedCharacters, MaxUrlLength))
                .Must(uri => Uri.IsWellFormedUriString(uri, UriKind.Absolute))
                .WithMessage(ErrorMessages.UrlMustBeValidAbsoluteUrl);
        }
    }
}