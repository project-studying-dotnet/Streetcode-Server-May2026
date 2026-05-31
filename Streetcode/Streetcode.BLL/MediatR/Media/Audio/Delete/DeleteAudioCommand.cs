using FluentResults;
using MediatR;
using Streetcode.BLL.MediatR.Interface;

namespace Streetcode.BLL.MediatR.Media.Audio.Delete;

public record DeleteAudioCommand(int Id) : IRequest<Result<Unit>>, IHasId;
