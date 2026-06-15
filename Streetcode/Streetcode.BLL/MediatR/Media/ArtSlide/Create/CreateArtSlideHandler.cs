using AutoMapper;
using FluentResults;
using MediatR;
using Streetcode.BLL.DTO.Media.ArtSlides;
using Streetcode.BLL.MediatR.Media.ArtSlide.Create;
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

        public async Task<Result<StreetcodeArtSlideDto>> Handle(CreateArtSlideCommand request, CancellationToken ct)
        {
            var streetcode = await _repositoryWrapper.StreetcodeRepository
                .GetFirstOrDefaultAsync(s => s.Id == request.Dto.StreetcodeId);

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
                _repositoryWrapper.StreetcodeArtSlideRepository.Create(newSlide);
                await _repositoryWrapper.SaveChangesAsync(ct);

                foreach (var artItem in request.Dto.ArtSlideItems)
                {
                    var artExists = await _repositoryWrapper.ArtRepository.GetFirstOrDefaultAsync(a => a.Id == artItem.ArtId);
                    if (artExists == null)
                    {
                        return Result.Fail($"Art with id {artItem.ArtId} does not exist.");
                    }
                    _repositoryWrapper.ArtSlideItemRepository.Create(new ArtSlideItem
                    {
                        SlideId = newSlide.Id,
                        ArtId = artItem.ArtId,
                        Index = artItem.Index
                    });
                }

                await _repositoryWrapper.SaveChangesAsync(ct);

                transaction.Complete();

                return Result.Ok(_mapper.Map<StreetcodeArtSlideDto>(newSlide));
            }
        }
    }
}