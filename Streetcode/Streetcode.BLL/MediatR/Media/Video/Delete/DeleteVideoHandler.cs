using AutoMapper;
using FluentResults;
using MediatR;
using Streetcode.BLL.DTO.Media.Video;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.DAL.Repositories.Interfaces.Base;

namespace Streetcode.BLL.MediatR.Media.Video.Delete
{
    public class DeleteVideoHandler : IRequestHandler<DeleteVideoCommand, Result<VideoDto>>
    {
        private readonly IMapper _mapper;
        private readonly IRepositoryWrapper _repositoryWrapper;
        private readonly ILoggerService _logger;

        public DeleteVideoHandler(IRepositoryWrapper repositoryWrapper, IMapper mapper, ILoggerService logger)
        {
            _repositoryWrapper = repositoryWrapper;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<Result<VideoDto>> Handle(DeleteVideoCommand request, CancellationToken cancellationToken)
        {
            var videoEntity = await _repositoryWrapper.VideoRepository
                .GetFirstOrDefaultAsync(v => v.Id == request.Id);

            if (videoEntity is null)
            {
                string errorMsg = $"Video with Id {request.Id} not found.";
                _logger.LogError(request, errorMsg);
                return Result.Fail(new Error(errorMsg));
            }

            _repositoryWrapper.VideoRepository.Delete(videoEntity);
            var saveResult = await _repositoryWrapper.SaveChangesAsync(cancellationToken);

            if (saveResult > 0)
            {
                return Result.Ok(_mapper.Map<VideoDto>(videoEntity));
            }

            string failMsg = $"Failed to delete Video with Id {request.Id}.";
            _logger.LogError(request, failMsg);
            return Result.Fail(new Error(failMsg));
        }
    }
}