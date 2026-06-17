using FluentResults;
using MediatR;

namespace Streetcode.BLL.MediatR.Comments.Delete;

public record DeleteCommentCommand(int CommentId) : IRequest<Result<bool>>;
