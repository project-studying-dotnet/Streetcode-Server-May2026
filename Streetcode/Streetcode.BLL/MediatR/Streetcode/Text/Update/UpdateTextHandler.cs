using AutoMapper;
using FluentResults;
using MediatR;
using Streetcode.BLL.DTO.Streetcode.TextContent.Text;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.DAL.Repositories.Interfaces.Base;
using T = Streetcode.DAL.Entities.Streetcode.TextContent;

namespace Streetcode.BLL.MediatR.Streetcode.Text.Update
{
    public class UpdateTextHandler : IRequestHandler<UpdateTextCommand, Result<TextDTO>>
    {
        private readonly IMapper _mapper;
        private readonly IRepositoryWrapper _repositoryWrapper;
        private readonly ILoggerService _logger;

        public UpdateTextHandler(IRepositoryWrapper repositoryWrapper, IMapper mapper, ILoggerService logger)
        {
            _repositoryWrapper = repositoryWrapper;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<Result<TextDTO>> Handle(UpdateTextCommand request, CancellationToken cancellationToken)
        {
            var textEntity = await _repositoryWrapper.TextRepository
                .GetFirstOrDefaultAsync(t => t.Id == request.updateTextRequest.Id);

            if (textEntity == null)
            {
                string errorMsg = $"Text with Id {request.updateTextRequest.Id} not found.";
                _logger.LogError(request, errorMsg);
                return Result.Fail<TextDTO>(errorMsg);
            }

            if (textEntity.StreetcodeId != request.updateTextRequest.StreetcodeId)
            {
                string errorMsg = "Changing StreetcodeId for an existing Text is not allowed.";
                _logger.LogError(request, errorMsg);
                return Result.Fail<TextDTO>(errorMsg);
            }

            _mapper.Map(request.updateTextRequest, textEntity);

            _repositoryWrapper.TextRepository.Update(textEntity);
            var saveResult = await _repositoryWrapper.SaveChangesAsync();

            if (saveResult > 0)
            {
                return Result.Ok(_mapper.Map<TextDTO>(textEntity));
            }

            const string failMsg = "Failed to update Text.";
            _logger.LogError(request, failMsg);
            return Result.Fail<TextDTO>(failMsg);
        }
    }
}