using AutoMapper;
using FluentResults;
using MediatR;
using Streetcode.BLL.Resources;
using Streetcode.BLL.DTO.Sources;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.DAL.Repositories.Interfaces.Base;

namespace Streetcode.BLL.MediatR.Sources.StreetcodeCategoryContent.Delete
{
    public class DeleteStreetcodeCategoryContentHandler
       : IRequestHandler<DeleteStreetcodeCategoryContentCommand, Result<StreetcodeCategoryContentDTO>>
    {
        private readonly IRepositoryWrapper _repositoryWrapper;
        private readonly IMapper _mapper;
        private readonly ILoggerService _logger;

        public DeleteStreetcodeCategoryContentHandler(
            IRepositoryWrapper repositoryWrapper,
            IMapper mapper,
            ILoggerService logger)
        {
            _repositoryWrapper = repositoryWrapper;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<Result<StreetcodeCategoryContentDTO>> Handle(
            DeleteStreetcodeCategoryContentCommand request,
            CancellationToken cancellationToken)
        {
            var content = await _repositoryWrapper.StreetcodeCategoryContentRepository
                .GetFirstOrDefaultAsync(
                    c => c.StreetcodeId == request.StreetcodeId &&
                         c.SourceLinkCategoryId == request.SourceLinkCategoryId,
                    cancellationToken: cancellationToken);

            if (content is null)
            {
                string errorMsg = ErrorMessages.SourceCategoryNotFound;
                _logger.LogError(request, errorMsg);
                return Result.Fail(new Error(errorMsg));
            }

            var contentDto = _mapper.Map<StreetcodeCategoryContentDTO>(content);

            _repositoryWrapper.StreetcodeCategoryContentRepository.Delete(content);

            var isSaved = await _repositoryWrapper.SaveChangesAsync(cancellationToken) > 0;

            if (!isSaved)
            {
                string errorMsg = ErrorMessages.CannotDeleteSourceCategoryСontent;
                _logger.LogError(request, errorMsg);
                return Result.Fail(new Error(errorMsg));
            }

            return Result.Ok(contentDto);
        }
    }
}
