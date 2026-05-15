using Streetcode.BLL.MediatR.AdditionalContent.GetById;
using Streetcode.BLL.MediatR.Media.Art.GetById;

namespace Streetcode.BLL.Validators.Media.Art.GetById
{
    /// <summary>
    /// Validator for GetArtByIdQuery.
    /// </summary>
    internal class GetArtByIdValidator
     : PositiveIdValidator<GetArtByIdQuery>
    {
    }
}
