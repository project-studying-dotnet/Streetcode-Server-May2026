using FluentResults;
using MediatR;
using Streetcode.BLL.DTO.Sources;
using System;
using System.Collections.Generic;
using System.Text;

namespace Streetcode.BLL.MediatR.Sources.StreetcodeCategoryContent.Create;

public record CreateStreetcodeCategoryContentCommand(CategoryContentCreateDTO CategoryContent)
    : IRequest<Result<StreetcodeCategoryContentDTO>>;
