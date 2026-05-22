using AutoMapper;
using FluentResults;
using MediatR;
using Streetcode.BLL.DTO.Streetcode.TextContent.Fact;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.DAL.Repositories.Interfaces.Base;

namespace Streetcode.BLL.MediatR.Streetcode.Fact.Delete;

public class DeleteFactHandler : IRequestHandler<DeleteFactCommand, Result<FactDto>>
{
    private readonly IRepositoryWrapper _repositoryWrapper;
    private readonly IMapper _mapper;
    private readonly ILoggerService _logger;

    public DeleteFactHandler(IRepositoryWrapper repositoryWrapper, IMapper mapper, ILoggerService logger)
    {
        _repositoryWrapper = repositoryWrapper;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<Result<FactDto>> Handle(DeleteFactCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var fact = await _repositoryWrapper.FactRepository.GetFirstOrDefaultAsync(f => f.Id == request.Id, cancellationToken: cancellationToken);

            if (fact is null)
            {
                string errorMsg = $"Cannot find fact with id: {request.Id}";
                _logger.LogError(request, errorMsg);
                return Result.Fail(new Error(errorMsg));
            }

            _repositoryWrapper.FactRepository.Delete(fact);

            var resultIsSuccess = await _repositoryWrapper.SaveChangesAsync(cancellationToken) > 0;
            var factDto = _mapper.Map<FactDto>(fact);

            if (resultIsSuccess && factDto != null)
            {
                return Result.Ok(factDto);
            }
            else
            {
                const string errorMsg = "Failed to delete fact";
                _logger.LogError(request, errorMsg);
                return Result.Fail(new Error(errorMsg));
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(request, ex.Message);
            return Result.Fail(new Error(ex.Message));
        }
    }
}
