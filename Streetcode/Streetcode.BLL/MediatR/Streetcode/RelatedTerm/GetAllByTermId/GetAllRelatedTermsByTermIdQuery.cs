using FluentResults;
using MediatR;
using Streetcode.BLL.DTO.Streetcode.TextContent;
using Streetcode.BLL.MediatR.Interface;

namespace Streetcode.BLL.MediatR.Streetcode.RelatedTerm.GetAllByTermId
{
    public record GetAllRelatedTermsByTermIdQuery(int Id) : IRequest<Result<IEnumerable<RelatedTermDTO>>>, IHasId
    {
    }
}
