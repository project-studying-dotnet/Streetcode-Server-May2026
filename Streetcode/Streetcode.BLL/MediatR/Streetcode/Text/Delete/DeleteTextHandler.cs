using AutoMapper;
using FluentResults;
using MediatR;
using Streetcode.BLL.DTO.Streetcode;
using Streetcode.BLL.DTO.Streetcode.TextContent.Text;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.DAL.Repositories.Interfaces.Base;

namespace Streetcode.BLL.MediatR.Streetcode.Text.Delete
{
    public class DeleteTextHandler : IRequestHandler<DeleteTextCommand, Result<TextDTO>>
    {
        private readonly IMapper _mapper;
        private readonly IRepositoryWrapper _repositoryWrapper;
        private readonly ILoggerService _logger;

        public DeleteTextHandler(IRepositoryWrapper repositoryWrapper, IMapper mapper, ILoggerService logger)
        {
            _repositoryWrapper = repositoryWrapper;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<Result<TextDTO>> Handle(DeleteTextCommand request, CancellationToken cancellationToken)
        {
            var textEntity = await _repositoryWrapper.TextRepository
                .GetFirstOrDefaultAsync(t => t.Id == request.Id);

            if (textEntity == null)
            {
                string errorMsg = $"Text with Id {request.Id} not found.";
                _logger.LogError(request, errorMsg);
                return Result.Fail<TextDTO>(errorMsg);
            }

            _repositoryWrapper.TextRepository.Delete(textEntity);
            var saveResult = await _repositoryWrapper.SaveChangesAsync();

            if (saveResult > 0)
            {
                return Result.Ok(_mapper.Map<TextDTO>(textEntity));
            }

            string failMsg = $"Failed to delete Text with Id {request.Id}.";
            _logger.LogError(request, failMsg);
            return Result.Fail<TextDTO>(failMsg);
        }
    }
}
