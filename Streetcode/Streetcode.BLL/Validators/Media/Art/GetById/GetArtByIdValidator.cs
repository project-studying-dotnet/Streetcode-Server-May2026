using Streetcode.BLL.MediatR.Media.Art.GetById;

namespace Streetcode.BLL.Validators.Media.Art.GetById
{
    /// <summary>
    /// Validator for GetArtByIdQuery.
    /// </summary>
    public class GetArtByIdValidator : PositiveIdValidator<GetArtByIdQuery>
    {
    }
}
