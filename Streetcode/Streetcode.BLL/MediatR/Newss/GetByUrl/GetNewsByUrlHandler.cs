using AutoMapper;
using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Streetcode.BLL.DTO.News;
using Streetcode.BLL.Interfaces.BlobStorage;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.DAL.Repositories.Interfaces.Base;
using Streetcode.BLL.Resources;

namespace Streetcode.BLL.MediatR.Newss.GetByUrl
{
    public class GetNewsByUrlHandler : NewsByUrlHandlerBase, IRequestHandler<GetNewsByUrlQuery, Result<NewsDTO>>
    {
        public GetNewsByUrlHandler(
            IMapper mapper,
            IRepositoryWrapper repositoryWrapper,
            IBlobService blobService,
            ILoggerService logger)
            : base(mapper, repositoryWrapper, blobService, logger)
        {
        }

        public async Task<Result<NewsDTO>> Handle(
            GetNewsByUrlQuery request,
            CancellationToken cancellationToken)
        {
            var newsDto = await GetNewsDtoByUrlAsync(request.Url);

            if (newsDto is null)
            {
                return FailNewsNotFound<NewsDTO>(request, request.Url);
            }

            FillImageBase64(newsDto);

            return Result.Ok(newsDto);
        }
    }
}
