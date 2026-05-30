using MediatR;
using FluentResults;
using Streetcode.BLL.DTO.Media.Video;

namespace Streetcode.BLL.MediatR.Media.Video.GetByStreetcodeId;

public record GetVideoByStreetcodeIdQuery(int StreetcodeId)
    : IRequest<Result<VideoDto>>;