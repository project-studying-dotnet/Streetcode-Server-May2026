using Streetcode.BLL.MediatR.Streetcode.Streetcode.GetById;

namespace Streetcode.BLL.Validators.Streetcode.Streetcode.GetById
{
    /// <summary>
    /// Validator for GetStreetcodeByIdQuery.
    /// </summary>
    public class GetStreetcodeByIdQueryValidator
        : PositiveIdValidator<GetStreetcodeByIdQuery>
    {
    }
}