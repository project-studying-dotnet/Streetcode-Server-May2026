using AutoMapper;
using FluentResults;
using Microsoft.EntityFrameworkCore;
using MediatR;
using Streetcode.BLL.DTO.Media.Video;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.DAL.Repositories.Interfaces.Base;
using T = Streetcode.DAL.Entities.Media;

namespace Streetcode.BLL.MediatR.Media.Video.Create
{
    public class CreateVideoHandler : IRequestHandler<CreateVideoCommand, Result<VideoDTO>>
    {
        private readonly IMapper _mapper;
        private readonly IRepositoryWrapper _repositoryWrapper;
        private readonly ILoggerService _logger;

        public CreateVideoHandler(IRepositoryWrapper repositoryWrapper, IMapper mapper, ILoggerService logger)
        {
            _repositoryWrapper = repositoryWrapper;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<Result<VideoDTO>> Handle(CreateVideoCommand request, CancellationToken cancellationToken)
        {
            var streetcodeId = request.createVideoRequest.StreetcodeId;

            bool streetcodeExists = await _repositoryWrapper.StreetcodeRepository
                .FindAll()
                .AnyAsync(s => s.Id == streetcodeId);

            if (!streetcodeExists)
            {
                string errorMsg = $"Streetcode with Id {streetcodeId} does not exist.";
                _logger.LogError(request, errorMsg);
                return Result.Fail<VideoDTO>(errorMsg);
            }

            bool videoAlreadyExists = await _repositoryWrapper.VideoRepository
                .FindAll()
                .AnyAsync(v => v.StreetcodeId == streetcodeId);

            if (videoAlreadyExists)
            {
                string errorMsg = $"Video for Streetcode Id {streetcodeId} already exists. Cannot create a duplicate.";
                _logger.LogError(request, errorMsg);
                return Result.Fail<VideoDTO>(errorMsg);
            }

            var videoEntity = _mapper.Map<T.Video>(request.createVideoRequest);

            if (videoEntity is null)
            {
                const string errorMsg = "Cannot map CreateVideoRequest to entity.";
                _logger.LogError(request, errorMsg);
                return Result.Fail<VideoDTO>(errorMsg);
            }

            await _repositoryWrapper.VideoRepository.CreateAsync(videoEntity);
            var saveResult = await _repositoryWrapper.SaveChangesAsync();

            if (saveResult > 0)
            {
                return Result.Ok(_mapper.Map<VideoDTO>(videoEntity));
            }

            const string failMsg = "Failed to save new Video.";
            _logger.LogError(request, failMsg);
            return Result.Fail<VideoDTO>(failMsg);
        }
    }
}
