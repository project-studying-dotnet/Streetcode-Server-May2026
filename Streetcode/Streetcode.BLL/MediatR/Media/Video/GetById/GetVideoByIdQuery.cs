using FluentResults;
using MediatR;
using Streetcode.BLL.DTO.Media.Video;
using Streetcode.BLL.MediatR.Interface;

namespace Streetcode.BLL.MediatR.Media.Video.GetById;

public record GetVideoByIdQuery(int Id)
    : IRequest<Result<VideoDto>>, IHasId;
