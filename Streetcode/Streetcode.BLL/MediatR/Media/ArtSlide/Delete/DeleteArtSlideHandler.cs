using FluentResults;
using MediatR;
using Streetcode.BLL.Resources;
using Streetcode.DAL.Repositories.Interfaces.Base;

namespace Streetcode.BLL.MediatR.Media.ArtSlide.Delete
{
    public class DeleteArtSlideHandler : IRequestHandler<DeleteArtSlideCommand, Result<Unit>>
    {
        private readonly IRepositoryWrapper _repositoryWrapper;

        public DeleteArtSlideHandler(IRepositoryWrapper repositoryWrapper)
        {
            _repositoryWrapper = repositoryWrapper;
        }

        public async Task<Result<Unit>> Handle(DeleteArtSlideCommand request, CancellationToken ct)
        {
            var slide = await _repositoryWrapper.StreetcodeArtSlideRepository
                .GetFirstOrDefaultAsync(s => s.Id == request.Id);

            if (slide == null)
            {
                return Result.Fail(string.Format(ErrorMessages.SlideNotFound, request.Id));
            }

            using (var transaction = _repositoryWrapper.BeginTransaction())
            {
                var items = await _repositoryWrapper.ArtSlideItemRepository
                    .GetAllAsync(i => i.SlideId == request.Id);

                _repositoryWrapper.ArtSlideItemRepository.DeleteRange(items);

                _repositoryWrapper.StreetcodeArtSlideRepository.Delete(slide);

                await _repositoryWrapper.SaveChangesAsync(ct);

                transaction.Complete();
            }

            return Result.Ok(Unit.Value);
        }
    }
}