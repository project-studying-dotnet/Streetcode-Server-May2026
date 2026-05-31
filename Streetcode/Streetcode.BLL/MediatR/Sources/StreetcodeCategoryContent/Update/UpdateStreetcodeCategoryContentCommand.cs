using FluentResults;
using MediatR;
using Streetcode.BLL.DTO.Sources;
using System;
using System.Collections.Generic;
using System.Text;

namespace Streetcode.BLL.MediatR.Sources.StreetcodeCategoryContent.Update;

public record UpdateStreetcodeCategoryContentCommand(
    CategoryContentUpdateDTO CategoryContent)
    : IRequest<Result<StreetcodeCategoryContentDTO>>;