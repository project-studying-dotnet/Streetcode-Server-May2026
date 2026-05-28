using FluentResults;
using MediatR;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.DAL.Repositories.Interfaces.Base;

namespace Streetcode.BLL.MediatR.Sources.SourceLinkCategory.Delete;

public class DeleteSourceLinkCategoryHandler
    : IRequestHandler<DeleteSourceLinkCategoryCommand, Result<int>>
{
    private readonly IRepositoryWrapper _repositoryWrapper;
    private readonly ILoggerService _logger;

    public DeleteSourceLinkCategoryHandler(
        IRepositoryWrapper repositoryWrapper,
        ILoggerService logger)
    {
        _repositoryWrapper = repositoryWrapper;
        _logger = logger;
    }

    public async Task<Result<int>> Handle(
        DeleteSourceLinkCategoryCommand request,
        CancellationToken cancellationToken)
    {
        var category = await _repositoryWrapper.SourceCategoryRepository
            .GetFirstOrDefaultAsync(c => c.Id == request.Id);

        if (category is null)
        {
            string errorMsg = "Category not found.";
            _logger.LogError(request, errorMsg);
            return Result.Fail(new Error(errorMsg));
        }

        _repositoryWrapper.SourceCategoryRepository.Delete(category);

        var isSaved = await _repositoryWrapper.SaveChangesAsync() > 0;

        if (!isSaved)
        {
            string errorMsg = "Cannot delete source category.";
            _logger.LogError(request, errorMsg);
            return Result.Fail(new Error(errorMsg));
        }

        return Result.Ok(request.Id);
    }
}