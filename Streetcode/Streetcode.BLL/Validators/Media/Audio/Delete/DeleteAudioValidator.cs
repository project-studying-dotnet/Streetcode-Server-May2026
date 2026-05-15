using Streetcode.BLL.MediatR.Media.Audio.Delete;

namespace Streetcode.BLL.Validators.Media.Audio.Delete
{
    /// <summary>
    /// Validator for DeleteAudioCommand.
    /// </summary>
    public class DeleteAudioValidator : PositiveIdValidator<DeleteAudioCommand>
    {
    }
}
