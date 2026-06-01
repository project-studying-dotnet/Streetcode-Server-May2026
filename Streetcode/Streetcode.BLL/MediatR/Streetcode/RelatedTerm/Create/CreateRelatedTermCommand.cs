using FluentResults;
using MediatR;
using Streetcode.BLL.DTO.Streetcode.TextContent;
using Streetcode.BLL.DTO.Streetcode.TextContent.RelatedTerm;

namespace Streetcode.BLL.MediatR.Streetcode.RelatedTerm.Create
{
    public record CreateRelatedTermCommand(CreateRelatedTermDto CreateRelatedTerm) : IRequest<Result<RelatedTermDTO>>
    {
    }
}
