using Streetcode.BLL.MediatR.AdditionalContent.GetById;
using Streetcode.BLL.MediatR.AdditionalContent.Tag.GetById;

namespace Streetcode.BLL.Validators.AdditionalContent.Tag.GetById
{
    /// <summary>
    /// Validator for GetTagByIdQuery.
    /// </summary>
    public class GetTagByIdValidator
     : PositiveIdValidator<GetTagByIdQuery>
    {
    }
}
