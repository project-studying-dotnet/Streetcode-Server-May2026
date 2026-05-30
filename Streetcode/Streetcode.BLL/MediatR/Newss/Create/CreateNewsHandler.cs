using AutoMapper;
using FluentResults;
using MediatR;
using Streetcode.BLL.DTO.News;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.DAL.Entities.News;
using Streetcode.DAL.Repositories.Interfaces.Base;
using Streetcode.BLL.Resources;

namespace Streetcode.BLL.MediatR.Newss.Create
{
    public class CreateNewsHandler : IRequestHandler<CreateNewsCommand, Result<NewsDTO>>
    {
        private readonly IMapper _mapper;
        private readonly IRepositoryWrapper _repositoryWrapper;
        private readonly ILoggerService _logger;
        public CreateNewsHandler(IMapper mapper, IRepositoryWrapper repositoryWrapper, ILoggerService logger)
        {
            _mapper = mapper;
            _repositoryWrapper = repositoryWrapper;
            _logger = logger;
        }

        public async Task<Result<NewsDTO>> Handle(CreateNewsCommand request, CancellationToken cancellationToken)
        {
            var news = _mapper.Map<News>(request.newNews);

            if (news is null)
            {
                string errorMsg = ErrorMessages.CannotConvertNullToNews;
                _logger.LogError(request, errorMsg);
                return Result.Fail(errorMsg);
            }

            if (news.ImageId == 0)
            {
                news.ImageId = null;
            }

            var entity = await _repositoryWrapper.NewsRepository.CreateAsync(news);

            var resultIsSuccess = await _repositoryWrapper.SaveChangesAsync(cancellationToken) > 0;

            if (resultIsSuccess)
            {
                return Result.Ok(_mapper.Map<NewsDTO>(entity));
            }
            else
            {
                string errorMsg = ErrorMessages.FailedToCreateNews;
                _logger.LogError(request, errorMsg);
                return Result.Fail(new Error(errorMsg));
            }
        }
    }
}
