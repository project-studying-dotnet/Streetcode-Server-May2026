using AutoMapper;
using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Streetcode.BLL.DTO.News;
using Streetcode.BLL.Interfaces.BlobStorage;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.DAL.Repositories.Interfaces.Base;
using Streetcode.BLL.Resources;

using NewsEntity = global::Streetcode.DAL.Entities.News.News;

namespace Streetcode.BLL.MediatR.Newss.GetNewsAndLinksByUrl
{
    public class GetNewsAndLinksByUrlHandler
    : NewsByUrlHandlerBase,
        IRequestHandler<GetNewsAndLinksByUrlQuery, Result<NewsDTOWithURLs>>
    {
        public GetNewsAndLinksByUrlHandler(
            IMapper mapper,
            IRepositoryWrapper repositoryWrapper,
            IBlobService blobService,
            ILoggerService logger)
            : base(mapper, repositoryWrapper, blobService, logger)
        {
        }

        public async Task<Result<NewsDTOWithURLs>> Handle(
            GetNewsAndLinksByUrlQuery request,
            CancellationToken cancellationToken)
        {
            var newsDto = await GetNewsDtoByUrlAsync(request.Url);

            if (newsDto is null)
            {
                return FailNewsNotFound<NewsDTOWithURLs>(request, request.Url);
            }

            FillImageBase64(newsDto);

            var news = (await RepositoryWrapper.NewsRepository.GetAllAsync()).ToList();
            var newsIndex = news.FindIndex(item => item.Id == newsDto.Id);

            var result = new NewsDTOWithURLs
            {
                News = newsDto,
                PrevNewsUrl = newsIndex > 0 ? news[newsIndex - 1].URL : null,
                NextNewsUrl = newsIndex < news.Count - 1 ? news[newsIndex + 1].URL : null,
                RandomNews = GetRandomNews(news, newsIndex),
            };

            return Result.Ok(result);
        }

        private static RandomNewsDTO GetRandomNews(
            List<NewsEntity> news,
            int newsIndex)
        {
            if (news.Count == 0)
            {
                return new RandomNewsDTO();
            }

            var randomIndex = GetRandomNewsIndex(news.Count, newsIndex);
            var randomNews = news[randomIndex];

            return new RandomNewsDTO
            {
                RandomNewsUrl = randomNews.URL,
                Title = randomNews.Title,
            };
        }

        private static int GetRandomNewsIndex(int newsCount, int currentIndex)
        {
            if (newsCount <= 1)
            {
                return currentIndex;
            }

            if (newsCount <= 3)
            {
                return currentIndex;
            }

            return currentIndex == newsCount - 1
                ? currentIndex - 2
                : newsCount - 1;
        }
    }
}
