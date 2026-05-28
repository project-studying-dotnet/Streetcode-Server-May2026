using AutoMapper;
using FluentResults;
using MediatR;
using Streetcode.BLL.DTO.Sources;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.DAL.Entities.Sources;
using Streetcode.DAL.Repositories.Interfaces.Base;
using SourceLinkCategoryEntity = Streetcode.DAL.Entities.Sources.SourceLinkCategory;

namespace Streetcode.BLL.MediatR.Sources.SourceLinkCategory.Create;

public class CreateSourceLinkCategoryHandler
    : IRequestHandler<CreateSourceLinkCategoryCommand, Result<SourceLinkCategoryDTO>>
{
    private readonly IMapper _mapper;
    private readonly IRepositoryWrapper _repositoryWrapper;
    private readonly ILoggerService _logger;

    public CreateSourceLinkCategoryHandler(
        IRepositoryWrapper repositoryWrapper,
        IMapper mapper,
        ILoggerService logger)
    {
        _repositoryWrapper = repositoryWrapper;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<Result<SourceLinkCategoryDTO>> Handle(
        CreateSourceLinkCategoryCommand request,
        CancellationToken cancellationToken)
    {
        var dto = request.Category;

        if (string.IsNullOrWhiteSpace(dto.Title))
        {
            string errorMsg = "Category title is required.";
            _logger.LogError(request, errorMsg);
            return Result.Fail(new Error(errorMsg));
        }

        if (dto.Title.Length > 23)
        {
            string errorMsg = "Category title must not exceed 23 characters.";
            _logger.LogError(request, errorMsg);
            return Result.Fail(new Error(errorMsg));
        }

        var existingCategory = await _repositoryWrapper.SourceCategoryRepository
            .GetFirstOrDefaultAsync(c => c.Title != null && c.Title.ToLower() == dto.Title.ToLower());

        if (existingCategory is not null)
        {
            string errorMsg = "Category with this title already exists.";
            _logger.LogError(request, errorMsg);
            return Result.Fail(new Error(errorMsg));
        }

        var category = _mapper.Map<SourceLinkCategoryEntity>(dto);

        await _repositoryWrapper.SourceCategoryRepository.CreateAsync(category);
        var isSaved = await _repositoryWrapper.SaveChangesAsync() > 0;

        if (!isSaved)
        {
            string errorMsg = "Cannot save source category.";
            _logger.LogError(request, errorMsg);
            return Result.Fail(new Error(errorMsg));
        }

        return Result.Ok(_mapper.Map<SourceLinkCategoryDTO>(category));
    }
}