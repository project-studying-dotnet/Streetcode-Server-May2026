using FluentResults;
using MediatR;
using Streetcode.BLL.DTO.Media.Audio;
using Streetcode.BLL.MediatR.Interface;

namespace Streetcode.BLL.MediatR.Media.Audio.GetById;

public record GetAudioByIdQuery(int Id) : IRequest<Result<AudioDTO>>, IHasId;
