using FluentResults;
using MediatR;
using Streetcode.BLL.DTO.Users;
using System;
using System.Collections.Generic;
using System.Text;

namespace Streetcode.BLL.MediatR.Users.Register
{
    public record RegisterUserCommand(UserRegisterDto registerRequest)
    : IRequest<Result<Unit>>;
}
