using AutoMapper;
using FluentResults;
using MediatR;
using Streetcode.BLL.DTO.Streetcode.TextContent.Text;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.Resources;
using Streetcode.DAL.Repositories.Interfaces.Base;
using T = Streetcode.DAL.Entities.Streetcode.TextContent;

namespace Streetcode.BLL.MediatR.Streetcode.Text.Update
{
    public class UpdateTextHandler : IRequestHandler<UpdateTextCommand, Result<TextDto>>
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

        public async Task<Result<TextDto>> Handle(UpdateTextCommand request, CancellationToken cancellationToken)
        {
            var textEntity = await _repositoryWrapper.TextRepository
                .GetFirstOrDefaultAsync(
                    predicate: t => t.Id == request.updateTextRequest.Id,
                    cancellationToken: cancellationToken);

            if (textEntity == null)
            {
                string errorMsg = string.Format(ErrorMessages.TextWithIdNotFound, request.updateTextRequest.Id);
                _logger.LogError(request, errorMsg);
                return Result.Fail<TextDto>(errorMsg);
            }

            if (textEntity.StreetcodeId != request.updateTextRequest.StreetcodeId)
            {
                string errorMsg = string.Format(ErrorMessages.ChangingStreetcodeIdNotAllowed, request.updateTextRequest.Id);
                _logger.LogError(request, errorMsg);
                return Result.Fail<TextDto>(errorMsg);
            }

            _mapper.Map(request.updateTextRequest, textEntity);

            _repositoryWrapper.TextRepository.Update(textEntity);
            var saveResult = await _repositoryWrapper.SaveChangesAsync(cancellationToken);

            if (saveResult > 0)
            {
                return Result.Ok(_mapper.Map<TextDto>(textEntity));
            }

            string failMsg = ErrorMessages.FailedToUpdateText;
            _logger.LogError(request, failMsg);
            return Result.Fail<TextDto>(failMsg);
        }
    }
}