using System;
using System.Collections.Generic;
using System.Text;
using FluentResults;
using MediatR;
using Streetcode.BLL.DTO.Streetcode;

namespace Streetcode.BLL.MediatR.Streetcode.Streetcode.Update
{
    public record UpdateStreetcodeCommand(StreetcodeDTO streetcode)
        : IRequest<Result<StreetcodeDTO>>;
}
