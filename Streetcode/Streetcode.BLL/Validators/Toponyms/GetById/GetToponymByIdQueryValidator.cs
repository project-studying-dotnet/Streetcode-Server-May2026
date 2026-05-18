using Streetcode.BLL.MediatR.Toponyms.GetById;

namespace Streetcode.BLL.Validators.Toponyms.GetById
{
    /// <summary>
    /// Validator for <see cref="GetToponymByIdQuery"/>.
    /// </summary>
    public class GetToponymByIdQueryValidator : PositiveIdValidator<GetToponymByIdQuery>
    {
    }
}
