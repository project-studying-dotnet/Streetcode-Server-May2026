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
    private readonly IMapper mapper;
    private readonly IRepositoryWrapper _repositoryWrapper;

    protected IRepositoryWrapper RepositoryWrapper => _repositoryWrapper;
    private readonly IBlobService blobService;
    private readonly ILoggerService logger;

    protected NewsByUrlHandlerBase(
        IMapper mapper,
        IRepositoryWrapper repositoryWrapper,
        IBlobService blobService,
        ILoggerService logger)
    {
        this.mapper = mapper;
        this._repositoryWrapper = repositoryWrapper;
        this.blobService = blobService;
        this.logger = logger;
    }

    protected async Task<NewsDTO?> GetNewsDtoByUrlAsync(string url)
    {
        var news = await _repositoryWrapper.NewsRepository.GetFirstOrDefaultAsync(
            predicate: newsEntity => newsEntity.URL == url,
            include: query => query.Include(newsEntity => newsEntity.Image!));

        return mapper.Map<NewsDTO>(news);
    }

    protected void FillImageBase64(NewsDTO newsDto)
    {
        if (newsDto.Image is null)
        {
            return;
        }

        newsDto.Image.Base64 = blobService.FindFileInStorageAsBase64(
            newsDto.Image.BlobName!);
    }

    protected Result<T> FailNewsNotFound<T>(object request, string url)
    {
        string errorMsg = string.Format(ErrorMessages.NoNewsFoundByUrl, url);

        logger.LogError(request, errorMsg);

        return Result.Fail(errorMsg);
    }
}