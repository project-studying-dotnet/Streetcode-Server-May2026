using AutoMapper;
using FluentResults;
using MediatR;
using Streetcode.BLL.DTO.Comments;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.Resources;
using Streetcode.DAL.Repositories.Interfaces.Base;

namespace Streetcode.BLL.MediatR.Comments.GetRepliesByCommentId;

public class GetRepliesByCommentIdHandler : IRequestHandler<GetRepliesByCommentIdQuery, Result<IEnumerable<CommentDto>>>
{
    private readonly IMapper _mapper;
    private readonly IRepositoryWrapper _repositoryWrapper;
    private readonly ILoggerService _logger;

    public GetRepliesByCommentIdHandler(IRepositoryWrapper repositoryWrapper, IMapper mapper, ILoggerService logger)
    {
        _repositoryWrapper = repositoryWrapper;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<Result<IEnumerable<CommentDto>>> Handle(GetRepliesByCommentIdQuery request, CancellationToken cancellationToken)
    {
        var commentId = request.CommentId;

        var parentComment = await _repositoryWrapper.CommentRepository
            .GetFirstOrDefaultAsync(c => c.Id == commentId, cancellationToken: cancellationToken);

        if (parentComment is null)
        {
            string errorMsg = string.Format(ErrorMessages.CommentWithIdNotFound, commentId);
            _logger.LogError(request, errorMsg);
            return Result.Fail<IEnumerable<CommentDto>>(errorMsg);
        }

        var replies = (await _repositoryWrapper.CommentRepository
            .GetAllAsync(c => c.ParentCommentId == commentId))
            .OrderBy(c => c.CreatedAt);

        return Result.Ok(_mapper.Map<IEnumerable<CommentDto>>(replies));
    }
}
