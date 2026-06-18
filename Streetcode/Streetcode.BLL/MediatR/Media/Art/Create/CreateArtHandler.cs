using AutoMapper;
using FluentResults;
using MediatR;
using Streetcode.BLL.DTO.Media.Art;
using Streetcode.DAL.Repositories.Interfaces.Base;

namespace Streetcode.BLL.MediatR.Media.Art.Create
{
    public class CreateArtHandler : IRequestHandler<CreateArtCommand, Result<ArtDTO>>
    {
        private readonly IMapper _mapper;
        private readonly IRepositoryWrapper _repositoryWrapper;

        public CreateArtHandler(IMapper mapper, IRepositoryWrapper repositoryWrapper)
        {
            _mapper = mapper;
            _repositoryWrapper = repositoryWrapper;
        }

        public async Task<Result<ArtDTO>> Handle(CreateArtCommand request, CancellationToken cancellationToken)
        {
            var art = _mapper.Map<DAL.Entities.Media.Images.Art>(request.ArtDto);
            await _repositoryWrapper.ArtRepository.CreateAsync(art);
            await _repositoryWrapper.SaveChangesAsync(cancellationToken);

            return Result.Ok(_mapper.Map<ArtDTO>(art));
        }
    }
}
