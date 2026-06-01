using FluentResults;
using MediatR;
using Streetcode.BLL.DTO.AdditionalContent.Subtitles;
using Streetcode.BLL.MediatR.Interface;

namespace Streetcode.BLL.MediatR.AdditionalContent.GetById;

public record GetSubtitleByIdQuery(int Id) : IRequest<Result<SubtitleDTO>>, IHasId;
