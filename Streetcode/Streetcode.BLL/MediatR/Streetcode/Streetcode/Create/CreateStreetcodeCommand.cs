using System;
using System.Collections.Generic;
using System.Text;
using FluentResults;
using MediatR;
using Streetcode.BLL.DTO.Streetcode;
using Streetcode.DAL.Entities.Streetcode;

namespace Streetcode.BLL.MediatR.Streetcode.Streetcode.Create
{
    public record CreateStreetcodeCommand(StreetcodeDTO newStreetcodeContent)
        : IRequest<Result<StreetcodeDTO>>;
}
