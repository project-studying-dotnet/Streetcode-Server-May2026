using FluentResults;
using MediatR;
using Streetcode.BLL.DTO.Sources;

namespace Streetcode.BLL.MediatR.Sources.StreetcodeCategoryContent.Delete;

public record DeleteStreetcodeCategoryContentCommand(
    int StreetcodeId,
    int SourceLinkCategoryId)
    : IRequest<Result<StreetcodeCategoryContentDTO>>;