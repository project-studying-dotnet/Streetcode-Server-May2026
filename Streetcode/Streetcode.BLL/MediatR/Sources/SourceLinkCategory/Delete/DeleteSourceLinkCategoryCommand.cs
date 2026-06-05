using FluentResults;
using MediatR;

namespace Streetcode.BLL.MediatR.Sources.SourceLinkCategory.Delete;

public record DeleteSourceLinkCategoryCommand(int Id)
    : IRequest<Result<int>>;