using Streetcode.BLL.MediatR.Media.Art.Delete;
using Streetcode.BLL.MediatR.Media.Image.Delete;

namespace Streetcode.BLL.Validators.Media.Art.Delete
{
    internal class DeleteArtCommandValidator : PositiveIdValidator<DeleteArtCommand>
    {
    }
}
