using FluentResults;
using MediatR;
using Streetcode.BLL.DTO.News;
using Streetcode.BLL.Validators.Newss;

namespace Streetcode.BLL.MediatR.Newss.GetNewsAndLinksByUrl
{
    public record GetNewsAndLinksByUrlQuery(string Url) : IRequest<Result<NewsDTOWithURLs>>, IUrlQuery;
}
