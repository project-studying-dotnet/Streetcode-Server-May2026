using AutoMapper;
using FluentResults;
using MediatR;
using Streetcode.BLL.DTO.Sources;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.Resources;
using Streetcode.DAL.Repositories.Interfaces.Base;

namespace Streetcode.BLL.MediatR.Sources.SourceLinkCategory.Update;

public class UpdateSourceLinkCategoryHandler
    : IRequestHandler<UpdateSourceLinkCategoryCommand, Result<SourceLinkCategoryDTO>>
{
    private readonly IMapper _mapper;
    private readonly IRepositoryWrapper _repositoryWrapper;
    private readonly ILoggerService _logger;

    public UpdateSourceLinkCategoryHandler(
        IRepositoryWrapper repositoryWrapper,
        IMapper mapper,
        ILoggerService logger)
    {
        _repositoryWrapper = repositoryWrapper;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<Result<SourceLinkCategoryDTO>> Handle(
        UpdateSourceLinkCategoryCommand request,
        CancellationToken cancellationToken)
    {
        var dto = request.Category;

        if (dto.Id <= 0)
        {
            string errorMsg = ErrorMessages.SourceCategoryIdRequired;
            _logger.LogError(request, errorMsg);
            return Result.Fail(new Error(errorMsg));
        }

        if (string.IsNullOrWhiteSpace(dto.Title))
        {
            string errorMsg = ErrorMessages.SourceCategoryTitleRequired;
            _logger.LogError(request, errorMsg);
            return Result.Fail(new Error(errorMsg));
        }

        if (dto.Title.Length > 23)
        {
            string errorMsg = ErrorMessages.SourceCategoryTitleTooLong;
            _logger.LogError(request, errorMsg);
            return Result.Fail(new Error(errorMsg));
        }

        if (dto.ImageId <= 0)
        {
            string errorMsg = ErrorMessages.SourceCategoryImageRequired;
            _logger.LogError(request, errorMsg);
            return Result.Fail(new Error(errorMsg));
        }

        var category = await _repositoryWrapper.SourceCategoryRepository
            .GetFirstOrDefaultAsync(c => c.Id == dto.Id);

        if (category is null)
        {
            string errorMsg = ErrorMessages.SourceCategoryNotFound;
            _logger.LogError(request, errorMsg);
            return Result.Fail(new Error(errorMsg));
        }

        var sameTitleCategory = await _repositoryWrapper.SourceCategoryRepository
            .GetFirstOrDefaultAsync(c =>
                c.Id != dto.Id &&
                c.Title != null &&
                c.Title.ToLower() == dto.Title.ToLower());

        if (sameTitleCategory is not null)
        {
            string errorMsg = ErrorMessages.SourceCategoryAlreadyExists;
            _logger.LogError(request, errorMsg);
            return Result.Fail(new Error(errorMsg));
        }

        category.Title = dto.Title;
        category.ImageId = dto.ImageId;

        _repositoryWrapper.SourceCategoryRepository.Update(category);

        var isSaved = await _repositoryWrapper.SaveChangesAsync(cancellationToken) > 0;

        if (!isSaved)
        {
            string errorMsg = ErrorMessages.CannotUpdateSourceCategory;
            _logger.LogError(request, errorMsg);
            return Result.Fail(new Error(errorMsg));
        }

        return Result.Ok(_mapper.Map<SourceLinkCategoryDTO>(category));
    }
}