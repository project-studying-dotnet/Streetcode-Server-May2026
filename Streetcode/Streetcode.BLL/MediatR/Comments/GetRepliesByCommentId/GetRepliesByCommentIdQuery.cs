using FluentResults;
using MediatR;
using Streetcode.BLL.DTO.Comments;

namespace Streetcode.BLL.MediatR.Comments.GetRepliesByCommentId;

public record GetRepliesByCommentIdQuery(int CommentId) : IRequest<Result<IEnumerable<CommentDto>>>;
