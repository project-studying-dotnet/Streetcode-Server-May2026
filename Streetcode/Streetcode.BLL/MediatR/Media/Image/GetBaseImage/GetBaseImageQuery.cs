using FluentResults;
using MediatR;
using Streetcode.BLL.MediatR.Interface;

namespace Streetcode.BLL.MediatR.Media.Image.GetBaseImage;

public record GetBaseImageQuery(int Id) : IRequest<Result<MemoryStream>>, IHasId;