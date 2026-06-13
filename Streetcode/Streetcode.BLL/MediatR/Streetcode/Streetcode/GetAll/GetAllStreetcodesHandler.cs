using AutoMapper;
using FluentResults;
using MediatR;
using Streetcode.BLL.DTO.Streetcode;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.DAL.Repositories.Interfaces.Base;
using Streetcode.DAL.Specifications.Streetcode;

namespace Streetcode.BLL.MediatR.Streetcode.Streetcode.GetAll;

public class GetAllStreetcodesHandler : IRequestHandler<GetAllStreetcodesQuery, Result<GetAllStreetcodesResponseDTO>>
{
    private readonly IMapper _mapper;
    private readonly IRepositoryWrapper _repositoryWrapper;
    private readonly ILoggerService _logger;

    public GetAllStreetcodesHandler(IRepositoryWrapper repositoryWrapper, IMapper mapper, ILoggerService logger)
    {
        _repositoryWrapper = repositoryWrapper;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<Result<GetAllStreetcodesResponseDTO>> Handle(GetAllStreetcodesQuery query, CancellationToken cancellationToken)
    {
        var filterRequest = query.request;

        var spec = new StreetcodePagedAndSortedSpecification(
            filterRequest.Page,
            filterRequest.Amount,
            filterRequest.Title,
            filterRequest.Sort,
            filterRequest.Filter);

        var totalCount = _repositoryWrapper.StreetcodeRepository.FindAll(spec.Criteria).Count();
        int pagesAmount = (int)Math.Ceiling(totalCount / (double)filterRequest.Amount);

        var streetcodes = await _repositoryWrapper.StreetcodeRepository.GetAllAsync(spec);

        var response = new GetAllStreetcodesResponseDTO
        {
            Pages = pagesAmount,
            Streetcodes = _mapper.Map<IEnumerable<StreetcodeDTO>>(streetcodes)
        };

        return Result.Ok(response);
    }
}