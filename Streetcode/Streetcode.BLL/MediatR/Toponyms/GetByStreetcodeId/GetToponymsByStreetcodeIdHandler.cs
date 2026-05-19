using MediatR;
using AutoMapper;
using FluentResults;
using Microsoft.EntityFrameworkCore;
using Streetcode.BLL.DTO.Toponyms;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.DAL.Entities.Toponyms;
using Streetcode.DAL.Repositories.Interfaces.Base;

#pragma warning disable SA1111 //Closing parenthesis should be on line of last parameter

namespace Streetcode.BLL.MediatR.Toponyms.GetByStreetcodeId;

public sealed class GetToponymsByStreetcodeIdHandler(
    IRepositoryWrapper repositoryWrapper,
    IMapper mapper,
    ILoggerService logger
) : IRequestHandler<GetToponymsByStreetcodeIdQuery, Result<IEnumerable<ToponymDTO>>>
{
    public async Task<Result<IEnumerable<ToponymDTO>>> Handle(GetToponymsByStreetcodeIdQuery request, CancellationToken cancellationToken)
    {
        List<Toponym> toponyms = await repositoryWrapper.ToponymRepository.FindAll(
            sc => sc.Streetcodes.Any(s => s.Id == request.StreetcodeId)
        ).GroupBy(t => t.StreetName).Select(
            g => g.First()
        ).ToListAsync(cancellationToken);

        if(toponyms.Count == 0)
        {
            string errorMsg = $"Cannot find any toponym by the streetcode id: {request.StreetcodeId}";
            logger.LogError(request, errorMsg);
            return Result.Fail(new Error(errorMsg));
        }

        return Result.Ok(mapper.Map<IEnumerable<ToponymDTO>>(toponyms));
    }
}