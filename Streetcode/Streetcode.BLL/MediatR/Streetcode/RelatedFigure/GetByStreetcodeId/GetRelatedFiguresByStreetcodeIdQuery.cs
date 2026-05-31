using FluentResults;
using MediatR;
using Streetcode.BLL.DTO.Streetcode.RelatedFigure;
using Streetcode.BLL.MediatR.Interface;

namespace Streetcode.BLL.MediatR.Streetcode.RelatedFigure.GetByStreetcodeId;

public record GetRelatedFigureByStreetcodeIdQuery(int StreetcodeId) : IRequest<Result<IEnumerable<RelatedFigureDTO>>>, IHasStreetcodeId;
