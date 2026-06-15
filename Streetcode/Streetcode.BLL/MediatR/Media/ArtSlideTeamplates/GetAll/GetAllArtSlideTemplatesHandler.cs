using AutoMapper;
using FluentResults;
using MediatR;
using Streetcode.BLL.Resources;
using Streetcode.DAL.Repositories.Interfaces.Base;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.DTO.Media.ArtSlidesTemplates;

namespace Streetcode.BLL.MediatR.Media.ArtSlideTeamplates.GetAll
{
    public class GetAllArtSlideTemplatesHandler : IRequestHandler<GetAllArtSlideTemplatesQuery, Result<IEnumerable<StreetcodeArtSlideTemplateDto>>>
    {
        private readonly IMapper _mapper;
        private readonly IRepositoryWrapper _repositoryWrapper;
        private readonly ILoggerService _logger;

        public GetAllArtSlideTemplatesHandler(IRepositoryWrapper repositoryWrapper, IMapper mapper, ILoggerService logger)
        {
            _repositoryWrapper = repositoryWrapper;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<Result<IEnumerable<StreetcodeArtSlideTemplateDto>>> Handle(GetAllArtSlideTemplatesQuery request, CancellationToken cancellationToken)
        {
            var templates = await _repositoryWrapper.StreetcodeArtSlideTemplateRepository.GetAllAsync();

            if (templates is null || !templates.Any())
            {
                string errorMsg = ErrorMessages.CannotFindAnyArtSlideTemplates;
                _logger.LogError(request, errorMsg);
                return Result.Fail(new Error(errorMsg));
            }

            var templateDtos = _mapper.Map<IEnumerable<StreetcodeArtSlideTemplateDto>>(templates);

            return Result.Ok(templateDtos);
        }
    }
}