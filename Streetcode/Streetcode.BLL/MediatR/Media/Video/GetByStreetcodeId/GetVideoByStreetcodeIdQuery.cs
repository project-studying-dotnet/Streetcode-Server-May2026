using FluentResults;
using MediatR;
using Streetcode.BLL.DTO.Media.Video;
using Streetcode.BLL.MediatR.Interface;

namespace Streetcode.BLL.MediatR.Media.Video.GetByStreetcodeId;

public record GetVideoByStreetcodeIdQuery(int StreetcodeId)
    : IRequest<Result<VideoDto>>, IHasStreetcodeId;