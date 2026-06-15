using AutoMapper;
using FluentResults;
using MediatR;
using Streetcode.BLL.DTO.News;
using Streetcode.BLL.Interfaces.BlobStorage;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.Resources;
using Streetcode.DAL.Repositories.Interfaces.Base;

using NewsEntity = Streetcode.DAL.Entities.News.News;

namespace Streetcode.BLL.MediatR.Newss.Update
{
    public class UpdateNewsHandler : IRequestHandler<UpdateNewsCommand, Result<NewsDTO>>
    {
        private readonly IRepositoryWrapper _repositoryWrapper;
        private readonly IMapper _mapper;
        private readonly IBlobService _blobSevice;
        private readonly ILoggerService _logger;

        public UpdateNewsHandler(
            IRepositoryWrapper repositoryWrapper,
            IMapper mapper,
            IBlobService blobService,
            ILoggerService logger)
        {
            _repositoryWrapper = repositoryWrapper;
            _mapper = mapper;
            _blobSevice = blobService;
            _logger = logger;
        }

        public async Task<Result<NewsDTO>> Handle(UpdateNewsCommand request, CancellationToken cancellationToken)
        {
            var updateNews = request.news;

            if (updateNews is null)
            {
                string errorMsg = ErrorMessages.CannotConvertNullToNews;
                _logger.LogError(request, errorMsg);
                return Result.Fail(new Error(errorMsg));
            }

            var newsEntity = await _repositoryWrapper.NewsRepository.GetFirstOrDefaultAsync(
                predicate: n => n.Id == updateNews.Id,
                cancellationToken: cancellationToken);

            if (newsEntity is null)
            {
                string errorMsg = $"News with id {updateNews.Id} was not found";
                _logger.LogError(request, errorMsg);
                return Result.Fail(new Error(errorMsg));
            }

            newsEntity.Title = updateNews.Title;
            newsEntity.Text = updateNews.Text;
            newsEntity.URL = updateNews.URL;
            newsEntity.CreationDate = updateNews.CreationDate;
            newsEntity.ImageId = updateNews.ImageId;

            _repositoryWrapper.NewsRepository.Update(newsEntity);

            var resultIsSuccess = await _repositoryWrapper.SaveChangesAsync(cancellationToken) > 0;

            if (!resultIsSuccess)
            {
                string errorMsg = ErrorMessages.FailedToUpdateNews;
                _logger.LogError(request, errorMsg);
                return Result.Fail(new Error(errorMsg));
            }

            var response = _mapper.Map<NewsDTO>(newsEntity);

            if (response.Image is not null)
            {
                response.Image.Base64 = _blobSevice.FindFileInStorageAsBase64(response.Image.BlobName!);
            }

            return Result.Ok(response);
        }
    }
}