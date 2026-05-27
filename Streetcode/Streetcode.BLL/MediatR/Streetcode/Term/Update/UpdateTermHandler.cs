using AutoMapper;
using FluentResults;
using MediatR;
using Streetcode.BLL.DTO.Streetcode.TextContent.Term;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.DAL.Repositories.Interfaces.Base;

namespace Streetcode.BLL.MediatR.Streetcode.Term.Update
{
    public class UpdateTermHandler : IRequestHandler<UpdateTermCommand, Result<TermDto>>
    {
        private readonly IRepositoryWrapper _repositoryWrapper;
        private readonly IMapper _mapper;
        private readonly ILoggerService _logger;

        public UpdateTermHandler(IRepositoryWrapper repositoryWrapper, IMapper mapper, ILoggerService logger)
        {
            _repositoryWrapper = repositoryWrapper;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<Result<TermDto>> Handle(UpdateTermCommand request, CancellationToken cancellationToken)
        {
            var termToUpdate = await _repositoryWrapper.TermRepository
                .GetFirstOrDefaultAsync(t => t.Id == request.Term.Id);

            if (termToUpdate is null)
            {
                var errorMsg = $"Cannot find a term with corresponding id: {request.Term.Id}";
                _logger.LogError(request, errorMsg);
                return Result.Fail(new Error(errorMsg));
            }

            if (!string.IsNullOrWhiteSpace(request.Term.Title))
            {
                termToUpdate.Title = request.Term.Title;
            }

            if (!string.IsNullOrWhiteSpace(request.Term.Description))
            {
                termToUpdate.Description = request.Term.Description;
            }

            _repositoryWrapper.TermRepository.Update(termToUpdate);
            var isSuccess = await _repositoryWrapper.SaveChangesAsync() > 0;

            if (isSuccess)
            {
                return Result.Ok(_mapper.Map<TermDto>(termToUpdate));
            }

            var failMsg = "Failed to update a term";
            _logger.LogError(request, failMsg);
            return Result.Fail(new Error(failMsg));
        }
    }
}
