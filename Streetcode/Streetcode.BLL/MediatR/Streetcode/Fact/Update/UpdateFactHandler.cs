using AutoMapper;
using FluentResults;
using MediatR;
using Streetcode.BLL.DTO.Streetcode.TextContent.Fact;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.DAL.Repositories.Interfaces.Base;

namespace Streetcode.BLL.MediatR.Streetcode.Fact.Update;

public class UpdateFactHandler : IRequestHandler<UpdateFactCommand, Result<Unit>>
{
    private readonly IMapper _mapper;
    private readonly IRepositoryWrapper _repositoryWrapper;
    private readonly ILoggerService _logger;

    public UpdateFactHandler(IRepositoryWrapper repositoryWrapper, IMapper mapper, ILoggerService logger)
    {
        _repositoryWrapper = repositoryWrapper;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<Result<Unit>> Handle(UpdateFactCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var fact = await _repositoryWrapper.FactRepository.GetFirstOrDefaultAsync(f => f.Id == request.Fact.Id, cancellationToken: cancellationToken);

            if (fact is null)
            {
                const string errorMsg = "Cannot find fact";
                _logger.LogError(request, errorMsg);
                return Result.Fail(new Error(errorMsg));
            }

            _mapper.Map(request.Fact, fact);

            _repositoryWrapper.FactRepository.Update(fact);
            var resultIsSuccess = await _repositoryWrapper.SaveChangesAsync(cancellationToken) > 0;

            if (!resultIsSuccess)
            {
                const string errorMsg = "Cannot save fact to database";
                _logger.LogError(request, errorMsg);
                return Result.Fail(new Error(errorMsg));
            }

            return Result.Ok(Unit.Value);
        }
        catch (Exception ex)
        {
            _logger.LogError(request, ex.Message);
            return Result.Fail(new Error(ex.Message));
        }
    }
}
