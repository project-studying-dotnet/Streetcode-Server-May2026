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
    : IRequestHandler<
        CreateStreetcodeCategoryContentCommand,
        Result<StreetcodeCategoryContentDTO>>
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
            var dto = request.CategoryContent;

            var category = await _repositoryWrapper.SourceCategoryRepository
                .GetFirstOrDefaultAsync(c => c.Id == dto.SourceLinkCategoryId);

            if (category is null)
            {
                string errorMsg = ErrorMessages.SourceCategoryNotFound;
                _logger.LogError(request, errorMsg);
                return Result.Fail(new Error(errorMsg));
            }

            var streetcode = await _repositoryWrapper.StreetcodeRepository
                .GetFirstOrDefaultAsync(s => s.Id == dto.StreetcodeId);

            if (streetcode is null)
            {
                string errorMsg = ErrorMessages.StreetcodeNotFound;
                _logger.LogError(request, errorMsg);
                return Result.Fail(new Error(errorMsg));
            }

            var existingContent = await _repositoryWrapper.StreetcodeCategoryContentRepository
                .GetFirstOrDefaultAsync(c =>
                    c.StreetcodeId == dto.StreetcodeId &&
                    c.SourceLinkCategoryId == dto.SourceLinkCategoryId);

            if (existingContent is not null)
            {
                string errorMsg = ErrorMessages.SourceCategoryAlreadyExists;
                _logger.LogError(request, errorMsg);
                return Result.Fail(new Error(errorMsg));
            }

            var content = _mapper.Map<StreetcodeCategoryContentEntity>(dto);

            await _repositoryWrapper.StreetcodeCategoryContentRepository.CreateAsync(content);

            var isSaved = await _repositoryWrapper.SaveChangesAsync(cancellationToken) > 0;

            if (!isSaved)
            {
                string errorMsg = ErrorMessages.CannotSaveSourceCategory;
                _logger.LogError(request, errorMsg);
                return Result.Fail(new Error(errorMsg));
            }

            return Result.Ok(_mapper.Map<StreetcodeCategoryContentDTO>(content));
        }

    }
}
