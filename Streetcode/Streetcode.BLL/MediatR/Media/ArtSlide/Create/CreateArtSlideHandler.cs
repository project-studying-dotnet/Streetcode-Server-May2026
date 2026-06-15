using AutoMapper;
using FluentResults;
using MediatR;
using Streetcode.BLL.DTO.Media.ArtSlides;
using Streetcode.BLL.Resources;
using Streetcode.DAL.Entities.Media.Images;
using Streetcode.DAL.Repositories.Interfaces.Base;

namespace Streetcode.BLL.MediatR.Media.ArtSlide.Create
{
    public class CreateArtSlideHandler : IRequestHandler<CreateArtSlideCommand, Result<StreetcodeArtSlideDto>>
    {
        private readonly IMapper _mapper;
        private readonly IRepositoryWrapper _repositoryWrapper;

        public CreateArtSlideHandler(IMapper mapper, IRepositoryWrapper repositoryWrapper)
        {
            _mapper = mapper;
            _repositoryWrapper = repositoryWrapper;
        }

        public async Task<Result<StreetcodeArtSlideDto>> Handle(CreateArtSlideCommand request, CancellationToken cancellationToken)
        {
            var streetcode = await _repositoryWrapper.StreetcodeRepository.GetFirstOrDefaultAsync(
                predicate: s => s.Id == request.Dto.StreetcodeId,
                cancellationToken: cancellationToken);

            if (streetcode == null)
            {
                return Result.Fail(string.Format(ErrorMessages.StreetcodeNotFound, request.Dto.StreetcodeId));
            }

            var artIdsFromDto = request.Dto.ArtSlideItems.Select(a => a.ArtId).Distinct().ToList();

            var existingArts = await _repositoryWrapper.ArtRepository
                .GetAllAsync(a => artIdsFromDto.Contains(a.Id));

            if (existingArts.Count() != artIdsFromDto.Count)
            {
                return Result.Fail(ErrorMessages.ArtsNotFound);
            }

            using (var transaction = _repositoryWrapper.BeginTransaction())
            {
                var newSlide = _mapper.Map<StreetcodeArtSlide>(request.Dto);
                await _repositoryWrapper.StreetcodeArtSlideRepository.CreateAsync(newSlide);
                await _repositoryWrapper.SaveChangesAsync(cancellationToken);

                foreach (var artItem in request.Dto.ArtSlideItems)
                {
                    var artExists = await _repositoryWrapper.ArtRepository.GetFirstOrDefaultAsync(
                        predicate: a => a.Id == artItem.ArtId,
                        cancellationToken: cancellationToken);

                    if (artExists == null)
                    {
                        return Result.Fail($"Art with id {artItem.ArtId} does not exist.");
                    }
                    await _repositoryWrapper.ArtSlideItemRepository.CreateAsync(new ArtSlideItem
                    {
                        SlideId = newSlide.Id,
                        ArtId = artItem.ArtId,
                        Index = artItem.Index
                    });
                }

                await _repositoryWrapper.SaveChangesAsync(cancellationToken);

                transaction.Complete();

                return Result.Ok(_mapper.Map<StreetcodeArtSlideDto>(newSlide));
            }
        }
    }
}