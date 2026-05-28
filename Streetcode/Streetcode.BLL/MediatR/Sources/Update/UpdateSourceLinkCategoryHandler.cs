using AutoMapper;
using FluentResults;
using MediatR;
using Streetcode.BLL.DTO.Sources;
using Streetcode.BLL.Interfaces.Logging;
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
            return Result.Fail(new Error("Category id is required."));
        }

        if (string.IsNullOrWhiteSpace(dto.Title))
        {
            return Result.Fail(new Error("Category title is required."));
        }

        if (dto.Title.Length > 23)
        {
            return Result.Fail(new Error("Category title must not exceed 23 characters."));
        }

        if (dto.ImageId <= 0)
        {
            return Result.Fail(new Error("Category image is required."));
        }

        var category = await _repositoryWrapper.SourceCategoryRepository
            .GetFirstOrDefaultAsync(c => c.Id == dto.Id);

        if (category is null)
        {
            return Result.Fail(new Error("Category not found."));
        }

        var sameTitleCategory = await _repositoryWrapper.SourceCategoryRepository
            .GetFirstOrDefaultAsync(c =>
                c.Id != dto.Id &&
                c.Title != null &&
                c.Title.ToLower() == dto.Title.ToLower());

        if (sameTitleCategory is not null)
        {
            return Result.Fail(new Error("Category with this title already exists."));
        }

        category.Title = dto.Title;
        category.ImageId = dto.ImageId;

        _repositoryWrapper.SourceCategoryRepository.Update(category);

        var isSaved = await _repositoryWrapper.SaveChangesAsync() > 0;

        if (!isSaved)
        {
            return Result.Fail(new Error("Cannot update source category."));
        }

        return Result.Ok(_mapper.Map<SourceLinkCategoryDTO>(category));
    }
}