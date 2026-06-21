using AutoMapper;
using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Streetcode.BLL.DTO.Media.ArtSlides;
using Streetcode.DAL.Repositories.Interfaces.Base;

namespace Streetcode.BLL.MediatR.Media.ArtSlide.GetAllByStreetcodeId
{
    public class GetAllArtSlidesByStreetcodeIdHandler : IRequestHandler<GetAllArtSlidesByStreetcodeIdQuery, Result<IEnumerable<StreetcodeArtSlideDto>>>
    {
        private readonly IMapper _mapper;
        private readonly IRepositoryWrapper _repositoryWrapper;

        public GetAllArtSlidesByStreetcodeIdHandler(IMapper mapper, IRepositoryWrapper repositoryWrapper)
        {
            _mapper = mapper;
            _repositoryWrapper = repositoryWrapper;
        }

        public async Task<Result<IEnumerable<StreetcodeArtSlideDto>>> Handle(GetAllArtSlidesByStreetcodeIdQuery request, CancellationToken cancellationToken)
        {
            var slides = await _repositoryWrapper.StreetcodeArtSlideRepository.GetAllAsync(
                predicate: s => s.StreetcodeId == request.StreetcodeId,
                include: q => q.Include(s => s.ArtSlideItems)
            );

            if (slides == null)
            {
                return Result.Fail("Slides not found");
            }

            return Result.Ok(_mapper.Map<IEnumerable<StreetcodeArtSlideDto>>(slides));
        }
    }
}