using System;
using System.Collections.Generic;
using System.Text;
using FluentResults;
using MediatR;
using Streetcode.BLL.DTO.Streetcode;

namespace Streetcode.BLL.MediatR.Streetcode.Streetcode.Delete
{
    public record DeleteStreetcodeCommand(int id)
        : IRequest<Result<StreetcodeDTO>>;
}
