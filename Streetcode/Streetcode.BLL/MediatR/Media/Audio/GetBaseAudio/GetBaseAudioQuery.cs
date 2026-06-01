using FluentResults;
using MediatR;
using Streetcode.BLL.MediatR.Interface;

namespace Streetcode.BLL.MediatR.Media.Audio.GetBaseAudio;

public record GetBaseAudioQuery(int Id) : IRequest<Result<MemoryStream>>, IHasId;
