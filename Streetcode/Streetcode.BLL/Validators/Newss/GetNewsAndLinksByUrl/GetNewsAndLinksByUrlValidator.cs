using FluentValidation;
using Streetcode.BLL.MediatR.Newss.GetNewsAndLinksByUrl;

namespace Streetcode.BLL.Validators.Newss.GetNewsAndLinksByUrl
{
    /// <summary>
    /// Validator for GetNewsAndLinksByUrlQuery.
    /// </summary>
    public class GetNewsAndLinksByUrlValidator : AbstractValidator<GetNewsAndLinksByUrlQuery>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GetNewsAndLinksByUrlValidator"/> class.
        /// </summary>
        public GetNewsAndLinksByUrlValidator()
        {
            RuleFor(x => x.url)
                .NotEmpty()
                .Must(uri => Uri.IsWellFormedUriString(uri, UriKind.Absolute))
                .WithMessage("Url must be a valid absolute URL");
        }
    }
}
