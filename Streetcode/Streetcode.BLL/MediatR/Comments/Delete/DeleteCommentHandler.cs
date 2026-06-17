using FluentResults;
using MediatR;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.Resources;
using Streetcode.DAL.Repositories.Interfaces.Base;

using CommentEntity = Streetcode.DAL.Entities.Comments.Comment;

namespace Streetcode.BLL.MediatR.Comments.Delete;

public class DeleteCommentHandler : IRequestHandler<DeleteCommentCommand, Result<bool>>
{
    private readonly IRepositoryWrapper _repositoryWrapper;
    private readonly ILoggerService _logger;

    public DeleteCommentHandler(IRepositoryWrapper repositoryWrapper, ILoggerService logger)
    {
        _repositoryWrapper = repositoryWrapper;
        _logger = logger;
    }

    public async Task<Result<bool>> Handle(DeleteCommentCommand request, CancellationToken cancellationToken)
    {
        var comment = await _repositoryWrapper.CommentRepository
            .GetFirstOrDefaultAsync(c => c.Id == request.CommentId, cancellationToken: cancellationToken);

        if (comment is null)
        {
            string errorMsg = string.Format(ErrorMessages.CommentWithIdNotFound, request.CommentId);
            _logger.LogError(request, errorMsg);
            return Result.Fail<bool>(errorMsg);
        }

        var allStreetcodeComments = (await _repositoryWrapper.CommentRepository
            .GetAllAsync(c => c.StreetcodeId == comment.StreetcodeId))
            .ToList();

        var descendants = GetAllDescendants(allStreetcodeComments, comment.Id);

        if (descendants.Count > 0)
        {
            _repositoryWrapper.CommentRepository.DeleteRange(descendants);
            await _repositoryWrapper.SaveChangesAsync(cancellationToken);
        }

        _repositoryWrapper.CommentRepository.Delete(comment);

        var resultIsSuccess = await _repositoryWrapper.SaveChangesAsync(cancellationToken) > 0;

        if (!resultIsSuccess)
        {
            string errorMsg = ErrorMessages.FailedToDeleteComment;
            _logger.LogError(request, errorMsg);
            return Result.Fail<bool>(errorMsg);
        }

        return Result.Ok(true);
    }

    // Returns descendants ordered deepest-first so FK constraints are satisfied on delete.
    private static List<CommentEntity> GetAllDescendants(List<CommentEntity> allComments, int parentId)
    {
        var descendants = new List<CommentEntity>();

        foreach (var child in allComments.Where(c => c.ParentCommentId == parentId))
        {
            descendants.AddRange(GetAllDescendants(allComments, child.Id));
            descendants.Add(child);
        }

        return descendants;
    }
}
