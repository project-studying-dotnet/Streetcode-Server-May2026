using AutoMapper;
using FluentResults;
using MediatR;
using Streetcode.BLL.Resources;
using Streetcode.DAL.Entities.Media.Images;
using Streetcode.DAL.Repositories.Interfaces.Base;

namespace Streetcode.BLL.MediatR.Media.ArtSlide.Update
{
    public class UpdateArtSlideHandler : IRequestHandler<UpdateArtSlideCommand, Result<Unit>>
    {
        private readonly IMapper _mapper;
        private readonly IRepositoryWrapper _repositoryWrapper;

        public UpdateArtSlideHandler(IMapper mapper, IRepositoryWrapper repositoryWrapper)
        {
            _mapper = mapper;
            _repositoryWrapper = repositoryWrapper;
        }

        public async Task<Result<Unit>> Handle(UpdateArtSlideCommand request, CancellationToken cancellationToken)
        {
            var slide = await _repositoryWrapper.StreetcodeArtSlideRepository.GetFirstOrDefaultAsync(
                predicate: s => s.Id == request.Dto.Id,
                cancellationToken: cancellationToken);

            if (slide == null)
            {
                return Result.Fail(string.Format(ErrorMessages.SlideNotFound, request.Dto.Id));
            }

            using (var transaction = _repositoryWrapper.BeginTransaction())
            {
                slide.Index = request.Dto.Index;
                slide.TemplateId = request.Dto.TemplateId;
                _repositoryWrapper.StreetcodeArtSlideRepository.Update(slide);

                var oldItems = await _repositoryWrapper.ArtSlideItemRepository
                    .GetAllAsync(i => i.SlideId == slide.Id);

                _repositoryWrapper.ArtSlideItemRepository.DeleteRange(oldItems);

                foreach (var item in request.Dto.ArtSlideItems)
                {
                   await _repositoryWrapper.ArtSlideItemRepository.CreateAsync(new ArtSlideItem
                    {
                        SlideId = slide.Id,
                        ArtId = item.ArtId,
                        Index = item.Index
                    });
                }

                await _repositoryWrapper.SaveChangesAsync(cancellationToken);
                transaction.Complete();
            }

            return Result.Ok(Unit.Value);
        }
    }
}