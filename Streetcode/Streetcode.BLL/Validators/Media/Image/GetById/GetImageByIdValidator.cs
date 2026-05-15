using Streetcode.BLL.MediatR.Media.Image.GetById;

namespace Streetcode.BLL.Validators.Media.Image.GetById
{
    /// <summary>
    /// Validator for GetImageByIdQuery.
    /// </summary>
    internal class GetImageByIdValidator
     : PositiveIdValidator<GetImageByIdQuery>
    {
    }
}
