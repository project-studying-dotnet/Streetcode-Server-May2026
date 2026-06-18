using AutoMapper;
using FluentResults;
using MediatR;
using Streetcode.BLL.DTO.Media.Art;
using Streetcode.BLL.Resources;
using Streetcode.DAL.Repositories.Interfaces.Base;

namespace Streetcode.BLL.MediatR.Media.Art.Update
{
    public class UpdateArtHandler : IRequestHandler<UpdateArtCommand, Result<ArtDTO>>
    {
        private readonly IMapper _mapper;
        private readonly IRepositoryWrapper _repositoryWrapper;

        public UpdateArtHandler(IMapper mapper, IRepositoryWrapper repositoryWrapper)
        {
            _mapper = mapper;
            _repositoryWrapper = repositoryWrapper;
        }

        public async Task<Result<ArtDTO>> Handle(UpdateArtCommand request, CancellationToken cancellationToken)
        {
            var art = await _repositoryWrapper.ArtRepository.GetFirstOrDefaultAsync(
                 predicate: a => a.Id == request.ArtDto.Id,
                 cancellationToken: cancellationToken
             );

            if (art == null)
            {
                return Result.Fail(string.Format(ErrorMessages.EntityNotFound, request.ArtDto.Id));
            }

            _mapper.Map(request.ArtDto, art);

            _repositoryWrapper.ArtRepository.Update(art);
            await _repositoryWrapper.SaveChangesAsync(cancellationToken);

            return Result.Ok(_mapper.Map<ArtDTO>(art));
        }
    }
}