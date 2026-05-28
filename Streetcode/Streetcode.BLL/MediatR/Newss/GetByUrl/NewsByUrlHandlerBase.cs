using AutoMapper;
using FluentResults;
using Microsoft.EntityFrameworkCore;
using Streetcode.BLL.DTO.News;
using Streetcode.BLL.Interfaces.BlobStorage;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.Resources;
using Streetcode.DAL.Repositories.Interfaces.Base;

namespace Streetcode.BLL.MediatR.Newss;

public abstract class NewsByUrlHandlerBase
{
    protected readonly IMapper Mapper;
    protected readonly IRepositoryWrapper RepositoryWrapper;
    protected readonly IBlobService BlobService;
    protected readonly ILoggerService Logger;

    protected NewsByUrlHandlerBase(
        IMapper mapper,
        IRepositoryWrapper repositoryWrapper,
        IBlobService blobService,
        ILoggerService logger)
    {
        Mapper = mapper;
        RepositoryWrapper = repositoryWrapper;
        BlobService = blobService;
        Logger = logger;
    }

    protected async Task<NewsDTO?> GetNewsDtoByUrlAsync(string url)
    {
        var news = await RepositoryWrapper.NewsRepository.GetFirstOrDefaultAsync(
            predicate: newsEntity => newsEntity.URL == url,
            include: query => query.Include(newsEntity => newsEntity.Image));

        return Mapper.Map<NewsDTO>(news);
    }

    protected void FillImageBase64(NewsDTO newsDto)
    {
        if (newsDto.Image is null)
        {
            return;
        }

        newsDto.Image.Base64 = BlobService.FindFileInStorageAsBase64(
            newsDto.Image.BlobName!);
    }

    protected Result<T> FailNewsNotFound<T>(object request, string url)
    {
        string errorMsg = string.Format(ErrorMessages.NoNewsFoundByUrl, url);

        Logger.LogError(request, errorMsg);

        return Result.Fail(errorMsg);
    }
}