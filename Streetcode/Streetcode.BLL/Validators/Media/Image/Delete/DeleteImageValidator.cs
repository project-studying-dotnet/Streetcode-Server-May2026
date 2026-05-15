using Streetcode.BLL.MediatR.Media.Image.Delete;

namespace Streetcode.BLL.Validators.Media.Image.Delete
{
    /// <summary>
    /// Validator for DeleteImageCommand.
    /// </summary>
    public class DeleteImageValidator : PositiveIdValidator<DeleteImageCommand>
    {
    }
}
