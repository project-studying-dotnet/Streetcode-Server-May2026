using FluentResults;
using MediatR;
using Streetcode.BLL.DTO.Partners;
using Streetcode.BLL.MediatR.Interface;

namespace Streetcode.BLL.MediatR.Partners.Delete
{
    public record DeletePartnerQuery(int Id) : IRequest<Result<PartnerDTO>>, IHasId;
}
