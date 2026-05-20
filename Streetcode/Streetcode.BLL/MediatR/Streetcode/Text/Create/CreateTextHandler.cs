using AutoMapper;
using FluentResults;
using MediatR;
using Streetcode.BLL.DTO.Streetcode.TextContent.Text;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.DAL.Repositories.Interfaces.Base;
using T = Streetcode.DAL.Entities.Streetcode.TextContent;

namespace Streetcode.BLL.MediatR.Streetcode.Text.Create;

public class CreateTextHandler : IRequestHandler<CreateTextCommand, Result<TextDTO>>
{
    private readonly IMapper _mapper;
    private readonly IRepositoryWrapper _repositoryWrapper;
    private readonly ILoggerService _logger;

    public CreateTextHandler(IRepositoryWrapper repositoryWrapper, IMapper mapper, ILoggerService logger)
    {
        _repositoryWrapper = repositoryWrapper;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<Result<TextDTO>> Handle(CreateTextCommand request, CancellationToken cancellationToken)
    {
        var streetcodeId = request.createTextRequest.StreetcodeId;

        var streetcodeExists = await _repositoryWrapper.StreetcodeRepository
            .GetFirstOrDefaultAsync(s => s.Id == streetcodeId);

        if (streetcodeExists == null)
        {
            string errorMsg = $"Streetcode with Id {streetcodeId} does not exist.";
            _logger.LogError(request, errorMsg);
            return Result.Fail<TextDTO>(errorMsg);
        }

        var existingText = await _repositoryWrapper.TextRepository
            .GetFirstOrDefaultAsync(t => t.StreetcodeId == streetcodeId);

        if (existingText != null)
        {
            string errorMsg = $"Text for Streetcode Id {streetcodeId} already exists. Cannot create a duplicate.";
            _logger.LogError(request, errorMsg);
            return Result.Fail<TextDTO>(errorMsg);
        }

        var textEntity = _mapper.Map<T.Text>(request.createTextRequest);

        if (textEntity == null)
        {
            const string errorMsg = "Cannot map CreateTextRequest to entity.";
            _logger.LogError(request, errorMsg);
            return Result.Fail<TextDTO>(errorMsg);
        }

        await _repositoryWrapper.TextRepository.CreateAsync(textEntity);
        var saveResult = await _repositoryWrapper.SaveChangesAsync();

        if (saveResult > 0)
        {
            return Result.Ok(_mapper.Map<TextDTO>(textEntity));
        }

        const string failMsg = "Failed to save new Text.";
        _logger.LogError(request, failMsg);
        return Result.Fail<TextDTO>(failMsg);
    }
}
