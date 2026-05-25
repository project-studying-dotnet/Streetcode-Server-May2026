using AutoMapper;
using FluentResults;
using MediatR;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.DTO.Streetcode.TextContent;
using Streetcode.DAL.Repositories.Interfaces.Base;
using Streetcode.BLL.Resources;

namespace Streetcode.BLL.MediatR.Streetcode.RelatedTerm.Delete
{
    public class DeleteRelatedTermHandler : IRequestHandler<DeleteRelatedTermCommand, Result<RelatedTermDTO>>
    {
        private readonly IRepositoryWrapper _repository;
        private readonly IMapper _mapper;
        private readonly ILoggerService _logger;

        public DeleteRelatedTermHandler(IRepositoryWrapper repository, IMapper mapper, ILoggerService logger)
        {
            _repository = repository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<Result<RelatedTermDTO>> Handle(DeleteRelatedTermCommand request, CancellationToken cancellationToken)
        {
            var relatedTerm = await _repository.RelatedTermRepository.GetFirstOrDefaultAsync(rt => rt.Word.ToLower().Equals(request.word.ToLower()));

            if (relatedTerm is null)
            {
                string errorMsg = string.Format(ErrorMessages.CannotFindRelatedTerm, request.word);
                _logger.LogError(request, errorMsg);
                return Result.Fail(new Error(errorMsg));
            }

            _repository.RelatedTermRepository.Delete(relatedTerm);

            var resultIsSuccess = await _repository.SaveChangesAsync() > 0;
            var relatedTermDto = _mapper.Map<RelatedTermDTO>(relatedTerm);
            if(resultIsSuccess && relatedTermDto != null)
            {
                return Result.Ok(relatedTermDto);
            }
            else
            {
                string errorMsg = ErrorMessages.FailedToDeleteRelatedTerm;
                _logger.LogError(request, errorMsg);
                return Result.Fail(new Error(errorMsg));
            }
        }
    }
}
