using AutoMapper;
using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Streetcode.BLL.DTO.Comments;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.Resources;
using Streetcode.DAL.Repositories.Interfaces.Base;

using CommentEntity = Streetcode.DAL.Entities.Comments.Comment;

namespace Streetcode.BLL.MediatR.Comments.GetByStreetcodeId;

public class GetCommentsByStreetcodeIdHandler : IRequestHandler<GetCommentsByStreetcodeIdQuery, Result<IEnumerable<CommentDto>>>
{
    private readonly IMapper _mapper;
    private readonly IRepositoryWrapper _repositoryWrapper;
    private readonly ILoggerService _logger;

    public GetCommentsByStreetcodeIdHandler(IRepositoryWrapper repositoryWrapper, IMapper mapper, ILoggerService logger)
    {
        _repositoryWrapper = repositoryWrapper;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<Result<IEnumerable<CommentDto>>> Handle(GetCommentsByStreetcodeIdQuery request, CancellationToken cancellationToken)
    {
        var streetcodeId = request.StreetcodeId;

        var streetcodeExists = await _repositoryWrapper.StreetcodeRepository
            .FindAll()
            .AnyAsync(s => s.Id == streetcodeId, cancellationToken);

        if (!streetcodeExists)
        {
            string errorMsg = string.Format(ErrorMessages.StreetcodeWithIdNotFound, streetcodeId);
            _logger.LogError(request, errorMsg);
            return Result.Fail<IEnumerable<CommentDto>>(errorMsg);
        }

        var allComments = (await _repositoryWrapper.CommentRepository
            .GetAllAsync(c => c.StreetcodeId == streetcodeId))
            .OrderBy(c => c.CreatedAt)
            .ToList();

        BuildReplyTree(allComments);

        var topLevelComments = allComments.Where(c => c.ParentCommentId == null);

        return Result.Ok(_mapper.Map<IEnumerable<CommentDto>>(topLevelComments));
    }

    private static void BuildReplyTree(List<CommentEntity> allComments)
    {
        foreach (var comment in allComments)
        {
            comment.Replies = allComments
                .Where(c => c.ParentCommentId == comment.Id)
                .ToList();
        }
    }
}
