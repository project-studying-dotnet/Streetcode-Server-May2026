using AutoMapper;
using FluentResults;
using MediatR;
using Streetcode.BLL.DTO.AdditionalContent.Subtitles;
using Streetcode.BLL.DTO.Streetcode.RelatedFigure;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.MediatR.Streetcode.Streetcode.GetByIndex;
using Streetcode.DAL.Entities.Streetcode.Types;
using Streetcode.DAL.Repositories.Interfaces.Base;
using Streetcode.DAL.Specifications.Streetcode;

namespace Streetcode.BLL.MediatR.Streetcode.RelatedFigure.GetByTagId
{
  internal class GetRelatedFiguresByTagIdHandler : IRequestHandler<GetRelatedFiguresByTagIdQuery, Result<IEnumerable<RelatedFigureDTO>>>
    {
        private readonly IMapper _mapper;
        private readonly IRepositoryWrapper _repositoryWrapper;
        private readonly ILoggerService _logger;

        public GetRelatedFiguresByTagIdHandler(IRepositoryWrapper repositoryWrapper, IMapper mapper, ILoggerService logger)
        {
            _repositoryWrapper = repositoryWrapper;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<Result<IEnumerable<RelatedFigureDTO>>> Handle(GetRelatedFiguresByTagIdQuery request, CancellationToken cancellationToken)
        {
            var streetcodes = await _repositoryWrapper.StreetcodeRepository
                .GetAllAsync(new StreetcodesByTagIdSpecification(request.tagId));

            if (streetcodes is null)
            {
                string errorMsg = $"Cannot find any streetcode with corresponding tagid: {request.tagId}";
                _logger.LogError(request, errorMsg);
                return Result.Fail(new Error(errorMsg));
            }

            return Result.Ok(_mapper.Map<IEnumerable<RelatedFigureDTO>>(streetcodes));
        }
    }
}
