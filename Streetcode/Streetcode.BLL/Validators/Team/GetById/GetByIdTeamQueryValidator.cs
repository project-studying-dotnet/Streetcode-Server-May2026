using Streetcode.BLL.MediatR.Team.GetById;

namespace Streetcode.BLL.Validators.Team.GetById
{
    /// <summary>
    /// Validator for <see cref="GetByIdTeamQuery"/>.
    /// </summary>
    public class GetByIdTeamQueryValidator : PositiveIdValidator<GetByIdTeamQuery>
    {
    }
}
