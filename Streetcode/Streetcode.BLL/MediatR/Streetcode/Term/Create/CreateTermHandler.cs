using AutoMapper;
using FluentResults;
using MediatR;
using Streetcode.BLL.DTO.Streetcode.TextContent;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.DAL.Repositories.Interfaces.Base;

using Entity = Streetcode.DAL.Entities.Streetcode.TextContent.Term;

namespace Streetcode.BLL.MediatR.Streetcode.Term.Create
{
    public class CreateTermHandler : IRequestHandler<CreateTermCommand, Result<TermDto>>
    {
        private readonly IRepositoryWrapper _repository;
        private readonly IMapper _mapper;
        private readonly ILoggerService _logger;

        public CreateTermHandler(IRepositoryWrapper repository, IMapper mapper, ILoggerService logger)
        {
            _repository = repository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<Result<TermDto>> Handle(CreateTermCommand request, CancellationToken cancellationToken)
        {
            var term = _mapper.Map<Entity>(request.Term);

            if (term is null)
            {
                const string errorMsg = "Cannot create new term!";
                _logger.LogError(request, errorMsg);
                return Result.Fail(new Error(errorMsg));
            }

            var existingTerms = await _repository.TermRepository
                .GetAllAsync(predicate: t => t.Title == request.Term.Title);

            if (existingTerms is null || existingTerms.Any())
            {
                const string errorMsg = "Термін з такою назвою вже існує у словнику";
                _logger.LogError(request, errorMsg);
                return Result.Fail(new Error(errorMsg));
            }

            var createdTerm = _repository.TermRepository.Create(term);

            var isSuccessResult = await _repository.SaveChangesAsync() > 0;

            if (!isSuccessResult)
            {
                const string errorMsg = "Cannot save changes in the database after term creation!";
                _logger.LogError(request, errorMsg);
                return Result.Fail(new Error(errorMsg));
            }

            var createdTermDTO = _mapper.Map<TermDto>(createdTerm);

            if (createdTermDTO != null)
            {
                return Result.Ok(createdTermDTO);
            }
            else
            {
                const string errorMsg = "Cannot map entity to DTO!";
                _logger.LogError(request, errorMsg);
                return Result.Fail(new Error(errorMsg));
            }
        }
    }
}
