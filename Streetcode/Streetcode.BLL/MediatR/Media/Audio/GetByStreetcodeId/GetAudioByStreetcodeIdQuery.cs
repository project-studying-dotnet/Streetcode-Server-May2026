using FluentResults;
using MediatR;
using Streetcode.BLL.DTO.Media.Audio;
using Streetcode.BLL.MediatR.Interface;

namespace Streetcode.BLL.MediatR.Media.Audio.GetByStreetcodeId;

public record GetAudioByStreetcodeIdQuery(int StreetcodeId) : IRequest<Result<AudioDTO>>, IHasStreetcodeId;