using AutoMapper;
using FluentResults;
using MediatR;
using Streetcode.BLL.DTO.Sources;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.Resources;
using Streetcode.DAL.Repositories.Interfaces.Base;
using SourceLinkCategoryEntity = Streetcode.DAL.Entities.Sources.SourceLinkCategory;

namespace Streetcode.BLL.MediatR.Sources.SourceLinkCategory.Create;

public class CreateSourceLinkCategoryHandler
    : IRequestHandler<CreateSourceLinkCategoryCommand, Result<SourceLinkCategoryDTO>>
{
    private readonly IMapper _mapper;
    private readonly IRepositoryWrapper _repositoryWrapper;
    private readonly ILoggerService _logger;

    public CreateSourceLinkCategoryHandler(
        IRepositoryWrapper repositoryWrapper,
        IMapper mapper,
        ILoggerService logger)
    {
        _repositoryWrapper = repositoryWrapper;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<Result<SourceLinkCategoryDTO>> Handle(
        CreateSourceLinkCategoryCommand request,
        CancellationToken cancellationToken)
    {
        var category = _mapper.Map<SourceLinkCategoryEntity>(request.Category);

        await _repositoryWrapper.SourceCategoryRepository.CreateAsync(category);

        var isSaved = await _repositoryWrapper.SaveChangesAsync(cancellationToken) > 0;

        if (!isSaved)
        {
            string errorMsg = ErrorMessages.CannotSaveSourceCategory;
            _logger.LogError(request, errorMsg);
            return Result.Fail(new Error(errorMsg));
        }

        _logger.LogInformation("Success! SourceLinkCategory was created.");

        return Result.Ok(_mapper.Map<SourceLinkCategoryDTO>(category));
    }
}