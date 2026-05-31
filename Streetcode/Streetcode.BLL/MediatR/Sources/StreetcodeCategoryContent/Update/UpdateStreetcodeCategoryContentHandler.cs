using AutoMapper;
using FluentResults;
using MediatR;
using Streetcode.BLL.DTO.Sources;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.Resources;
using Streetcode.DAL.Repositories.Interfaces.Base;

namespace Streetcode.BLL.MediatR.Sources.StreetcodeCategoryContent.Update;

public class UpdateStreetcodeCategoryContentHandler
    : IRequestHandler<
        UpdateStreetcodeCategoryContentCommand,
        Result<StreetcodeCategoryContentDTO>>
{
    private readonly IRepositoryWrapper _repositoryWrapper;
    private readonly IMapper _mapper;
    private readonly ILoggerService _logger;

    public UpdateStreetcodeCategoryContentHandler(
        IRepositoryWrapper repositoryWrapper,
        IMapper mapper,
        ILoggerService logger)
    {
        _repositoryWrapper = repositoryWrapper;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<Result<StreetcodeCategoryContentDTO>> Handle(
        UpdateStreetcodeCategoryContentCommand request,
        CancellationToken cancellationToken)
    {
        var dto = request.CategoryContent;

        var existingContent = await _repositoryWrapper.StreetcodeCategoryContentRepository
            .GetFirstOrDefaultAsync(c =>
                c.StreetcodeId == dto.StreetcodeId &&
                c.SourceLinkCategoryId == dto.SourceLinkCategoryId);

        if (existingContent is null)
        {
            string errorMsg = ErrorMessages.SourceCategoryNotFound;
            _logger.LogError(request, errorMsg);
            return Result.Fail(new Error(errorMsg));
        }

        existingContent.Text = dto.Text;

        _repositoryWrapper.StreetcodeCategoryContentRepository.Update(existingContent);

        var isSaved = await _repositoryWrapper.SaveChangesAsync(cancellationToken) > 0;

        if (!isSaved)
        {
            string errorMsg = ErrorMessages.CannotUpdateSourceCategory;
            _logger.LogError(request, errorMsg);
            return Result.Fail(new Error(errorMsg));
        }

        return Result.Ok(_mapper.Map<StreetcodeCategoryContentDTO>(existingContent));
    }
}