using AutoMapper;
using FluentResults;
using MediatR;
using Streetcode.BLL.DTO.Comments;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.Resources;
using Streetcode.DAL.Repositories.Interfaces.Base;

using CommentEntity = Streetcode.DAL.Entities.Comments.Comment;

namespace Streetcode.BLL.MediatR.Comments.Update;

public class UpdateCommentHandler : IRequestHandler<UpdateCommentCommand, Result<CommentDto>>
{
    private readonly IMapper _mapper;
    private readonly IRepositoryWrapper _repositoryWrapper;
    private readonly ILoggerService _logger;

    public UpdateCommentHandler(IRepositoryWrapper repositoryWrapper, IMapper mapper, ILoggerService logger)
    {
        _repositoryWrapper = repositoryWrapper;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<Result<CommentDto>> Handle(UpdateCommentCommand request, CancellationToken cancellationToken)
    {
        var updateComment = request.UpdateComment;

        var commentEntity = await _repositoryWrapper.CommentRepository
            .GetFirstOrDefaultAsync(c => c.Id == updateComment.Id, cancellationToken: cancellationToken);

        if (commentEntity is null)
        {
            string errorMsg = string.Format(ErrorMessages.CommentWithIdNotFound, updateComment.Id);
            _logger.LogError(request, errorMsg);
            return Result.Fail<CommentDto>(errorMsg);
        }

        if (commentEntity.StreetcodeId != updateComment.StreetcodeId)
        {
            string errorMsg = ErrorMessages.ChangingCommentStreetcodeIdNotAllowed;
            _logger.LogError(request, errorMsg);
            return Result.Fail<CommentDto>(errorMsg);
        }

        if (updateComment.ParentCommentId is int parentCommentId)
        {
            var parentValidationResult = await ValidateParentCommentAsync(
                request,
                updateComment.Id,
                parentCommentId,
                updateComment.StreetcodeId,
                cancellationToken);

            if (parentValidationResult is not null)
            {
                return parentValidationResult;
            }
        }

        _mapper.Map(updateComment, commentEntity);
        commentEntity.UpdatedAt = DateTime.UtcNow;

        _repositoryWrapper.CommentRepository.Update(commentEntity);
        var resultIsSuccess = await _repositoryWrapper.SaveChangesAsync(cancellationToken) > 0;

        if (!resultIsSuccess)
        {
            string errorMsg = ErrorMessages.FailedToUpdateComment;
            _logger.LogError(request, errorMsg);
            return Result.Fail<CommentDto>(errorMsg);
        }

        return Result.Ok(_mapper.Map<CommentDto>(commentEntity));
    }

    private async Task<Result<CommentDto>?> ValidateParentCommentAsync(
        UpdateCommentCommand request,
        int commentId,
        int parentCommentId,
        int streetcodeId,
        CancellationToken cancellationToken)
    {
        if (parentCommentId == commentId)
        {
            string errorMsg = ErrorMessages.CommentCannotBeItsOwnParent;
            _logger.LogError(request, errorMsg);
            return Result.Fail<CommentDto>(errorMsg);
        }

        var parentComment = await _repositoryWrapper.CommentRepository
            .GetFirstOrDefaultAsync(c => c.Id == parentCommentId, cancellationToken: cancellationToken);

        if (parentComment is null)
        {
            string errorMsg = string.Format(ErrorMessages.ParentCommentNotFound, parentCommentId);
            _logger.LogError(request, errorMsg);
            return Result.Fail<CommentDto>(errorMsg);
        }

        if (parentComment.StreetcodeId != streetcodeId)
        {
            string errorMsg = ErrorMessages.ParentCommentMustBelongToSameStreetcode;
            _logger.LogError(request, errorMsg);
            return Result.Fail<CommentDto>(errorMsg);
        }

        return await ValidateNoCircularParentReferenceAsync(request, commentId, parentComment, cancellationToken);
    }

    private async Task<Result<CommentDto>?> ValidateNoCircularParentReferenceAsync(
        UpdateCommentCommand request,
        int commentId,
        CommentEntity parentComment,
        CancellationToken cancellationToken)
    {
        var currentParentId = parentComment.ParentCommentId;

        while (currentParentId is int ancestorId)
        {
            if (ancestorId == commentId)
            {
                string errorMsg = ErrorMessages.ParentCommentCannotBeDescendantOfComment;
                _logger.LogError(request, errorMsg);
                return Result.Fail<CommentDto>(errorMsg);
            }

            var ancestorComment = await _repositoryWrapper.CommentRepository
                .GetFirstOrDefaultAsync(c => c.Id == ancestorId, cancellationToken: cancellationToken);

            if (ancestorComment is null)
            {
                break;
            }

            currentParentId = ancestorComment.ParentCommentId;
        }

        return null;
    }
}
