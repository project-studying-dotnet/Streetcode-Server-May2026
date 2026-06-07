using AutoMapper;
using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
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
            if (parentCommentId == updateComment.Id)
            {
                string errorMsg = ErrorMessages.CommentCannotBeItsOwnParent;
                _logger.LogError(request, errorMsg);
                return Result.Fail<CommentDto>(errorMsg);
            }

            var parentComment = await _repositoryWrapper.CommentRepository
                .FindAll()
                .FirstOrDefaultAsync(c => c.Id == parentCommentId, cancellationToken);

            if (parentComment is null)
            {
                string errorMsg = string.Format(ErrorMessages.ParentCommentNotFound, parentCommentId);
                _logger.LogError(request, errorMsg);
                return Result.Fail<CommentDto>(errorMsg);
            }

            if (parentComment.StreetcodeId != updateComment.StreetcodeId)
            {
                string errorMsg = ErrorMessages.ParentCommentMustBelongToSameStreetcode;
                _logger.LogError(request, errorMsg);
                return Result.Fail<CommentDto>(errorMsg);
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
}
