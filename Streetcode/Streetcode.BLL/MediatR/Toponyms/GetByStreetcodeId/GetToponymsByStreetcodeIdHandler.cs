using AutoMapper;
using AutoMapper.QueryableExtensions;
using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Streetcode.BLL.DTO.Toponyms;
using Microsoft.EntityFrameworkCore;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.DAL.Repositories.Interfaces.Base;
using AutoMapper.QueryableExtensions;

#pragma warning disable SA1111 //Closing parenthesis should be on line of last parameter
#pragma warning disable SA1513 //Closing brace should be followed by blank line

namespace Streetcode.BLL.MediatR.Toponyms.GetByStreetcodeId;

public class GetToponymsByStreetcodeIdHandler : IRequestHandler<GetToponymsByStreetcodeIdQuery, Result<IEnumerable<ToponymDTO>>>
{
    private readonly IMapper _mapper;
    private readonly IRepositoryWrapper _repositoryWrapper;
    private readonly ILoggerService _logger;

    public GetToponymsByStreetcodeIdHandler(IRepositoryWrapper repositoryWrapper, IMapper mapper, ILoggerService logger)
    {
        _repositoryWrapper = repositoryWrapper;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<Result<IEnumerable<ToponymDTO>>> Handle(GetToponymsByStreetcodeIdQuery request, CancellationToken cancellationToken)
    {
        List<ToponymDTO> toponyms = _repositoryWrapper.ToponymRepository.FindAll(
            sc => sc.Streetcodes.Any(s => s.Id == request.StreetcodeId)
        ).DistinctBy(t => t.StreetName).ProjectTo<ToponymDTO>(_mapper.ConfigurationProvider).ToList();

        if (toponyms.Count == 0)
        {
            string errorMsg = $"Cannot find any toponym by the streetcode id: {request.StreetcodeId}";
            _logger.LogError(request, errorMsg);
            return Result.Fail(new Error(errorMsg));
        }
        return Result.Ok(toponyms.AsEnumerable());
    }
}