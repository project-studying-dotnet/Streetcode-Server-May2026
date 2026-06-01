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
            var word = request.Word?.ToLower();

            var relatedTerm = await _repository.RelatedTermRepository.GetFirstOrDefaultAsync(
                predicate: rt =>
                    rt.Word != null &&
                    rt.Word.ToLower().Equals(word) &&
                    rt.TermId == request.TermId,
                cancellationToken: cancellationToken);

            if (relatedTerm is null)
            {
                string errorMsg = string.Format(ErrorMessages.CannotFindRelatedTerm, request.Word);
                _logger.LogError(request, errorMsg);
                return Result.Fail(new Error(errorMsg));
            }

            _repository.RelatedTermRepository.Delete(relatedTerm);

            var resultIsSuccess = await _repository.SaveChangesAsync(cancellationToken) > 0;
            var relatedTermDto = _mapper.Map<RelatedTermDTO>(relatedTerm);

            if (resultIsSuccess && relatedTermDto != null)
            {
                return Result.Ok(relatedTermDto);
            }

            string failedErrorMsg = ErrorMessages.FailedToDeleteRelatedTerm;
            _logger.LogError(request, failedErrorMsg);
            return Result.Fail(new Error(failedErrorMsg));
        }
    }
}
