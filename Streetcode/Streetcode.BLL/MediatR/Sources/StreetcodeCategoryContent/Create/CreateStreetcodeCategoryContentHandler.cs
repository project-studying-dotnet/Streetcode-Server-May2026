using AutoMapper;
using FluentResults;
using MediatR;
using Streetcode.BLL.Resources;
using Streetcode.BLL.DTO.Sources;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.DAL.Repositories.Interfaces.Base;
using StreetcodeCategoryContentEntity = Streetcode.DAL.Entities.Sources.StreetcodeCategoryContent;

namespace Streetcode.BLL.MediatR.Sources.StreetcodeCategoryContent.Create
{
    public class CreateStreetcodeCategoryContentHandler
       : IRequestHandler<CreateStreetcodeCategoryContentCommand, Result<StreetcodeCategoryContentDTO>>
    {
        private readonly IRepositoryWrapper _repositoryWrapper;
        private readonly IMapper _mapper;
        private readonly ILoggerService _logger;

        public CreateStreetcodeCategoryContentHandler(
            IRepositoryWrapper repositoryWrapper,
            IMapper mapper,
            ILoggerService logger)
        {
            _repositoryWrapper = repositoryWrapper;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<Result<StreetcodeCategoryContentDTO>> Handle(
            CreateStreetcodeCategoryContentCommand request,
            CancellationToken cancellationToken)
        {
            var content = _mapper.Map<StreetcodeCategoryContentEntity>(request.CategoryContent);

            await _repositoryWrapper.StreetcodeCategoryContentRepository.CreateAsync(content);

            var isSaved = await _repositoryWrapper.SaveChangesAsync(cancellationToken) > 0;

            if (!isSaved)
            {
                string errorMsg = ErrorMessages.CannotSaveSourceCategoryContent;
                _logger.LogError(request, errorMsg);
                return Result.Fail(new Error(errorMsg));
            }

            return Result.Ok(_mapper.Map<StreetcodeCategoryContentDTO>(content));
        }
    }
}
