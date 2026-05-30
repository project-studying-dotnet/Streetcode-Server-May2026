using System;
using System.Collections.Generic;
using System.Text;
using AutoMapper;
using FluentResults;
using MediatR;
using Streetcode.BLL.DTO.Streetcode;
using Streetcode.BLL.Interfaces.BlobStorage;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.DAL.Repositories.Interfaces.Base;
using Streetcode.DAL.Entities.Streetcode;

namespace Streetcode.BLL.MediatR.Streetcode.Streetcode.Update
{
    public class UpdateStreetcodeHandler : IRequestHandler<UpdateStreetcodeCommand, Result<StreetcodeDTO>>
    {
        private readonly IMapper _mapper;
        private readonly IRepositoryWrapper _repositoryWrapper;
        private readonly ILoggerService _logger;

        public UpdateStreetcodeHandler(IMapper mapper, IRepositoryWrapper repositoryWrapper, ILoggerService loggerService)
        {
            _mapper = mapper;
            _repositoryWrapper = repositoryWrapper;
            _logger = loggerService;
        }

        public async Task<Result<StreetcodeDTO>> Handle(UpdateStreetcodeCommand request, CancellationToken cancellationToken)
        {
            var entity = await _repositoryWrapper.StreetcodeRepository
                    .GetFirstOrDefaultAsync(
                        predicate: s => s.Id == request.streetcode.Id,
                        cancellationToken: cancellationToken);

            if (entity is null)
            {
                string errorMsg = $"Streetcode with ID {request.streetcode.Id} not found";
                _logger.LogError(request, errorMsg);
                return Result.Fail(new Error(errorMsg));
            }

            var streetcode = _mapper.Map<StreetcodeContent>(request.streetcode);

            if (streetcode is null)
            {
                const string errorMsg = $"Cannot convert null to streetcode";
                _logger.LogError(request, errorMsg);
                return Result.Fail(new Error(errorMsg));
            }

            _repositoryWrapper.StreetcodeRepository.Update(streetcode);

            var resultIsSuccess = await _repositoryWrapper.SaveChangesAsync(cancellationToken) > 0;

            if (resultIsSuccess)
            {
                var response = _mapper.Map<StreetcodeDTO>(streetcode);
                return Result.Ok(response);
            }
            else
            {
                string errorMsg = $"Failed to update streetcode with ID {request.streetcode.Id}";
                _logger.LogError(request, errorMsg);
                return Result.Fail(new Error(errorMsg));
            }
        }
    }
}
