using FluentResults;
using MediatR;
using Streetcode.BLL.DTO.Sources;
using Streetcode.BLL.MediatR.Interface;

namespace Streetcode.BLL.MediatR.Sources.SourceLink.GetCategoryById;

public record GetCategoryByIdQuery(int Id) : IRequest<Result<SourceLinkCategoryDto>>, IHasId;