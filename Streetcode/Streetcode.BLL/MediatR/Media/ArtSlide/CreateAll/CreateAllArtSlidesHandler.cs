using AutoMapper;
using FluentResults;
using MediatR;
using Streetcode.BLL.DTO.Media.ArtSlides;
using Streetcode.BLL.MediatR.Media.ArtSlide.CreateAll;
using Streetcode.BLL.Resources;
using Streetcode.DAL.Entities.Media.Images;
using Streetcode.DAL.Repositories.Interfaces.Base;

namespace Streetcode.BLL.MediatR.Media.ArtSlide.Create
{
    public class CreateAllArtSlidesHandler : IRequestHandler<CreateAllArtSlidesCommand, Result<IEnumerable<StreetcodeArtSlideDto>>>
    {
        private readonly IMapper _mapper;
        private readonly IRepositoryWrapper _repositoryWrapper;

        public CreateAllArtSlidesHandler(IMapper mapper, IRepositoryWrapper repositoryWrapper)
        {
            _mapper = mapper;
            _repositoryWrapper = repositoryWrapper;
        }

        public async Task<Result<IEnumerable<StreetcodeArtSlideDto>>> Handle(
              CreateAllArtSlidesCommand request,
              CancellationToken cancellationToken)
        {
            if (request.ArtSlides == null || request.ArtSlides.Count == 0)
            {
                return Result.Fail("ArtSlides list is empty");
            }

            var streetcodeId = request.ArtSlides.First().StreetcodeId;

            var streetcode = await _repositoryWrapper.StreetcodeRepository.GetFirstOrDefaultAsync(
                 predicate: s => s.Id == streetcodeId,
                 cancellationToken: cancellationToken);

            if (streetcode == null)
            {
                return Result.Fail(
                    string.Format(ErrorMessages.StreetcodeNotFound, streetcodeId));
            }

            var resultList = new List<StreetcodeArtSlideDto>();

            using (var transaction = _repositoryWrapper.BeginTransaction())
            {
                foreach (var dto in request.ArtSlides)
                {
                    if (dto.StreetcodeId != streetcodeId)
                    {
                        return Result.Fail("All slides must belong to the same StreetcodeId");
                    }

                    var newSlide = _mapper.Map<StreetcodeArtSlide>(dto);

                    await _repositoryWrapper.StreetcodeArtSlideRepository.CreateAsync(newSlide);
                    await _repositoryWrapper.SaveChangesAsync(cancellationToken);

                    foreach (var artItem in dto.ArtSlideItems)
                    {
                        _repositoryWrapper.ArtSlideItemRepository.Create(
                            new ArtSlideItem
                            {
                                SlideId = newSlide.Id,
                                ArtId = artItem.ArtId,
                                Index = artItem.Index
                            });
                    }

                    await _repositoryWrapper.SaveChangesAsync(cancellationToken);

                    resultList.Add(_mapper.Map<StreetcodeArtSlideDto>(newSlide));
                }

                transaction.Complete();
            }

            return Result.Ok(resultList.AsEnumerable());
        }
    }
}