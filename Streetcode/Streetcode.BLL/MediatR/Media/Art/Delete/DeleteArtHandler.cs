using FluentResults;
using MediatR;
using Streetcode.BLL.Resources;
using Streetcode.DAL.Repositories.Interfaces.Base;

namespace Streetcode.BLL.MediatR.Media.Art.Delete
{
    public class DeleteArtHandler : IRequestHandler<DeleteArtCommand, Result<Unit>>
    {
        private readonly IRepositoryWrapper _repositoryWrapper;

        public DeleteArtHandler(IRepositoryWrapper repositoryWrapper)
        {
            _repositoryWrapper = repositoryWrapper;
        }

        public async Task<Result<Unit>> Handle(DeleteArtCommand request, CancellationToken ct)
        {
            var art = await _repositoryWrapper.ArtRepository
                .GetFirstOrDefaultAsync(a => a.Id == request.Id);

            if (art == null)
            {
                return Result.Fail(string.Format(ErrorMessages.EntityNotFound, request.Id));
            }

            _repositoryWrapper.ArtRepository.Delete(art);

            await _repositoryWrapper.SaveChangesAsync(ct);

            return Result.Ok(Unit.Value);
        }
    }
}