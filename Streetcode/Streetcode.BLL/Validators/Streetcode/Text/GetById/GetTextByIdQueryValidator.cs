using Streetcode.BLL.MediatR.Streetcode.Text.GetById;

namespace Streetcode.BLL.Validators.Streetcode.Text.GetById
{
    /// <summary>
    /// Validator for <see cref="GetTextByIdQuery"/>.
    /// </summary>
    public class GetTextByIdQueryValidator : PositiveIdValidator<GetTextByIdQuery>
    {
    }
}