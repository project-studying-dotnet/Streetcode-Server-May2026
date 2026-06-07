using AutoMapper;
using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Streetcode.BLL.DTO.Comments;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.Resources;
using Streetcode.DAL.Repositories.Interfaces.Base;

using CommentEntity = Streetcode.DAL.Entities.Comments.Comment;

namespace Streetcode.BLL.MediatR.Comments.Create;

public class CreateCommentHandler : IRequestHandler<CreateCommentCommand, Result<CommentDto>>
{
    private readonly IMapper _mapper;
    private readonly IRepositoryWrapper _repositoryWrapper;
    private readonly ILoggerService _logger;

    public CreateCommentHandler(IRepositoryWrapper repositoryWrapper, IMapper mapper, ILoggerService logger)
    {
        _repositoryWrapper = repositoryWrapper;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<Result<CommentDto>> Handle(CreateCommentCommand request, CancellationToken cancellationToken)
    {
        var streetcodeId = request.CreateComment.StreetcodeId;

        var streetcodeExists = await _repositoryWrapper.StreetcodeRepository
            .FindAll()
            .AnyAsync(s => s.Id == streetcodeId, cancellationToken);

        if (!streetcodeExists)
        {
            string errorMsg = string.Format(ErrorMessages.StreetcodeWithIdNotFound, streetcodeId);
            _logger.LogError(request, errorMsg);
            return Result.Fail<CommentDto>(errorMsg);
        }

        if (request.CreateComment.ParentCommentId is int parentCommentId)
        {
            var parentComment = await _repositoryWrapper.CommentRepository
                .FindAll()
                .FirstOrDefaultAsync(c => c.Id == parentCommentId, cancellationToken);

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
        }

        var commentEntity = _mapper.Map<CommentEntity>(request.CreateComment);

        if (commentEntity is null)
        {
            string errorMsg = ErrorMessages.CannotMapEntity;
            _logger.LogError(request, errorMsg);
            return Result.Fail<CommentDto>(errorMsg);
        }

        await _repositoryWrapper.CommentRepository.CreateAsync(commentEntity);
        var resultIsSuccess = await _repositoryWrapper.SaveChangesAsync(cancellationToken) > 0;

        if (!resultIsSuccess)
        {
            string errorMsg = ErrorMessages.CannotSaveCommentToDatabase;
            _logger.LogError(request, errorMsg);
            return Result.Fail<CommentDto>(errorMsg);
        }

        var commentDto = _mapper.Map<CommentDto>(commentEntity);

        if (commentDto is null)
        {
            string errorMsg = ErrorMessages.CannotMapEntity;
            _logger.LogError(request, errorMsg);
            return Result.Fail<CommentDto>(errorMsg);
        }

        return Result.Ok(commentDto);
    }
}
