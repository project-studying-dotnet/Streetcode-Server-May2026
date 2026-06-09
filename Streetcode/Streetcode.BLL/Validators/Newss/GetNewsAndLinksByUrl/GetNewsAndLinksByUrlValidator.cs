using FluentValidation;
using Streetcode.BLL.MediatR.Newss.GetNewsAndLinksByUrl;
using Streetcode.BLL.Resources;

namespace Streetcode.BLL.Validators.Newss.GetNewsAndLinksByUrl
{
    public class GetNewsAndLinksByUrlValidator : BaseNewsUrlValidator<GetNewsAndLinksByUrlQuery> { }
}