using Streetcode.BLL.MediatR.Streetcode.Text.GetByStreetcodeId;

namespace Streetcode.BLL.Validators.Streetcode.Text.GetByStreetcodeId
{
    /// <summary>
    /// Validator for <see cref="GetTextByStreetcodeIdQuery"/>.
    /// </summary>
    public class GetTextByStreetcodeIdQueryValidator : PositiveStreetcodeIdValidator<GetTextByStreetcodeIdQuery>
    {
    }
}