using AutoMapper;
using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Rewrite;
using Streetcode.BLL.DTO.AdditionalContent.Subtitles;
using Streetcode.BLL.DTO.Sources;
using Streetcode.BLL.DTO.Transactions;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.MediatR.ResultVariations;
using Streetcode.BLL.Resources;
using Streetcode.DAL.Repositories.Interfaces.Base;

namespace Streetcode.BLL.MediatR.Transactions.TransactionLink.GetByStreetcodeId;

public class GetTransactLinkByStreetcodeIdHandler : IRequestHandler<GetTransactLinkByStreetcodeIdQuery, Result<TransactLinkDTO?>>
{
    private readonly IMapper _mapper;
    private readonly IRepositoryWrapper _repositoryWrapper;
    private readonly ILoggerService _logger;
    public GetTransactLinkByStreetcodeIdHandler(IRepositoryWrapper repositoryWrapper, IMapper mapper, ILoggerService logger)
    {
        _repositoryWrapper = repositoryWrapper;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<Result<TransactLinkDTO?>> Handle(GetTransactLinkByStreetcodeIdQuery request, CancellationToken cancellationToken)
    {
        var transactLink = await _repositoryWrapper.TransactLinksRepository
            .GetFirstOrDefaultAsync(
                predicate: f => f.StreetcodeId == request.StreetcodeId,
                cancellationToken: cancellationToken);

        if (transactLink is null)
        {
            if (await _repositoryWrapper.StreetcodeRepository
                .GetFirstOrDefaultAsync(
                    predicate: s => s.Id == request.StreetcodeId,
                    cancellationToken: cancellationToken) == null)
            {
                string errorMsg = string.Format(ErrorMessages.CannotFindAnyTransactionLinkWithCorrespondingStreetcodeId, request.StreetcodeId);
                _logger.LogError(request, errorMsg);
                return Result.Fail(new Error(errorMsg));
            }
        }

        NullResult<TransactLinkDTO?> result = new NullResult<TransactLinkDTO?>();
        result.WithValue(_mapper.Map<TransactLinkDTO?>(transactLink));
        return result;
    }
}