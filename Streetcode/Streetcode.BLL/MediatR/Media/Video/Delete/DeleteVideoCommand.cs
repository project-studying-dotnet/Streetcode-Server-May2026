using FluentResults;
using MediatR;
using Streetcode.BLL.DTO.Media.Video;

namespace Streetcode.BLL.MediatR.Media.Video.Delete
{
    public record DeleteVideoCommand(int Id) : IRequest<Result<VideoDTO>>;
}
