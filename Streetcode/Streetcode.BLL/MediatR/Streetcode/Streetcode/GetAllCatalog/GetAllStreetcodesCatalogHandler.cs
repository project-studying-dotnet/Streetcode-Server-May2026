using AutoMapper;
using FluentResults;
using MediatR;
using Streetcode.BLL.DTO.Streetcode.RelatedFigure;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.DAL.Repositories.Interfaces.Base;
using Streetcode.DAL.Specifications.Streetcode;

namespace Streetcode.BLL.MediatR.Streetcode.Streetcode.GetAllCatalog
{
    public class GetAllStreetcodesCatalogHandler : IRequestHandler<GetAllStreetcodesCatalogQuery,
        Result<IEnumerable<RelatedFigureDTO>>>
    {
        private readonly IMapper _mapper;
        private readonly IRepositoryWrapper _repositoryWrapper;
        private readonly ILoggerService _logger;

        public GetAllStreetcodesCatalogHandler(IRepositoryWrapper repositoryWrapper, IMapper mapper, ILoggerService logger)
        {
            _repositoryWrapper = repositoryWrapper;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<Result<IEnumerable<RelatedFigureDTO>>> Handle(GetAllStreetcodesCatalogQuery request, CancellationToken cancellationToken)
        {
            var spec = new StreetcodeForCatalogSpecification(request.page, request.count);
            var streetcodes = await _repositoryWrapper.StreetcodeRepository.GetAllAsync(spec);

            if (streetcodes != null)
            {
                return Result.Ok(_mapper.Map<IEnumerable<RelatedFigureDTO>>(streetcodes));
            }

            const string errorMsg = $"Cannot find any subtitles";
            _logger.LogError(request, errorMsg);
            return Result.Fail(errorMsg);
        }
    }
}
