using Streetcode.BLL.MediatR.Newss.Delete;

namespace Streetcode.BLL.Validators.Newss.Delete
{
    /// <summary>
    /// Validator for DeleteNewsCommand.
    /// </summary>
    public class DeleteNewsValidator : PositiveIdValidator<DeleteNewsCommand>
    {
    }
}