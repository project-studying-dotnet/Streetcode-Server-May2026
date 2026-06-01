using FluentResults;
using MediatR;
using Streetcode.BLL.DTO.Sources;
using Streetcode.BLL.MediatR.Interface;

namespace Streetcode.BLL.MediatR.Sources.SourceLink.GetCategoriesByStreetcodeId;

public record GetCategoriesByStreetcodeIdQuery(int StreetcodeId) : IRequest<Result<IEnumerable<SourceLinkCategoryDTO>>>, IHasStreetcodeId;