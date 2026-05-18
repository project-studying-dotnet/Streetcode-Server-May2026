using FluentResults;
using MediatR;
using Streetcode.BLL.DTO.News;
using Streetcode.BLL.MediatR.Interface;

namespace Streetcode.BLL.MediatR.Newss.GetById
{
    public record GetNewsByIdQuery(int Id) : IRequest<Result<NewsDTO>>, IHasId;
}
