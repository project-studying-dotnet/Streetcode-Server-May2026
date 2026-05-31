using FluentResults;
using MediatR;
using Streetcode.BLL.DTO.Sources;
using System;
using System.Collections.Generic;
using System.Text;

namespace Streetcode.BLL.MediatR.Sources.StreetcodeCategoryContent.Delete;

public record DeleteStreetcodeCategoryContentCommand(
    int StreetcodeId,
    int SourceLinkCategoryId)
    : IRequest<Result<StreetcodeCategoryContentDTO>>;