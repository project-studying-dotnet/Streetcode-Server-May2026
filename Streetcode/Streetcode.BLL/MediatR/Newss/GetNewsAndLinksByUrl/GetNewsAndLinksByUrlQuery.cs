using FluentResults;
using MediatR;
using Streetcode.BLL.DTO.News;

namespace Streetcode.BLL.MediatR.Newss.GetNewsAndLinksByUrl
{
    public record GetNewsAndLinksByUrlQuery(string Url) : IRequest<Result<NewsDTOWithURLs>>, IUrlQuery;
}
