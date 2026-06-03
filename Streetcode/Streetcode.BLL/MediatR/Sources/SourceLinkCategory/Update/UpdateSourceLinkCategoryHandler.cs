using AutoMapper;
using FluentResults;
using MediatR;
using Streetcode.BLL.DTO.Sources;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.Resources;
using Streetcode.DAL.Repositories.Interfaces.Base;

namespace Streetcode.BLL.MediatR.Sources.SourceLinkCategory.Update;

public class UpdateSourceLinkCategoryHandler
    : IRequestHandler<UpdateSourceLinkCategoryCommand, Result<SourceLinkCategoryDto>>
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

    public async Task<Result<SourceLinkCategoryDto>> Handle(
        UpdateSourceLinkCategoryCommand request,
        CancellationToken cancellationToken)
    {
        var dto = request.Category;

        var category = await _repositoryWrapper.SourceCategoryRepository
            .GetFirstOrDefaultAsync(
            c => c.Id == dto.Id,
            cancellationToken: cancellationToken);

        if (category is null)
        {
            string errorMsg = ErrorMessages.SourceCategoryNotFound;
            _logger.LogError(request, errorMsg);
            return Result.Fail(new Error(errorMsg));
        }

        category.Title = dto.Title;
        category.ImageId = dto.ImageId;

        _repositoryWrapper.SourceCategoryRepository.Update(category);

        var isSaved =
            await _repositoryWrapper.SaveChangesAsync(cancellationToken) > 0;

        if (!isSaved)
        {
            string errorMsg = ErrorMessages.CannotUpdateSourceCategory;
            _logger.LogError(request, errorMsg);
            return Result.Fail(new Error(errorMsg));
        }

        return Result.Ok(_mapper.Map<SourceLinkCategoryDto>(category));
    }
}