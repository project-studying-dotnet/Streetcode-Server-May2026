using FluentResults;
using MediatR;
using Streetcode.BLL.DTO.Media.Images;
using Streetcode.BLL.MediatR.Interface;

namespace Streetcode.BLL.MediatR.Media.Image.GetById;

public record GetImageByIdQuery(int Id) : IRequest<Result<ImageDTO>>, IHasId;
