using System;
using System.Collections.Generic;
using System.Text;
using AutoMapper;
using FluentResults;
using MediatR;
using Streetcode.BLL.DTO.Streetcode;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.DAL.Entities.Streetcode;
using Streetcode.DAL.Repositories.Interfaces.Base;

namespace Streetcode.BLL.MediatR.Streetcode.Streetcode.Create
{
    public class CreateStreetcodeHandler : IRequestHandler<CreateStreetcodeCommand, Result<StreetcodeDTO>>
    {
        private readonly IMapper _mapper;
        private readonly IRepositoryWrapper _repositoryWrapper;
        private readonly ILoggerService _logger;

        public CreateStreetcodeHandler(IMapper mapper, IRepositoryWrapper repositoryWrapper, ILoggerService logger)
        {
            _mapper = mapper;
            _repositoryWrapper = repositoryWrapper;
            _logger = logger;
        }

        public async Task<Result<StreetcodeDTO>> Handle(CreateStreetcodeCommand request, CancellationToken cancellationToken)
        {
            var newStreetcode = _mapper.Map<StreetcodeContent>(request.newStreetcodeContent);

            if (newStreetcode is null)
            {
                const string errorMsg = "Cannot convert null to streetcode";
                _logger.LogError(request, errorMsg);
                return Result.Fail(errorMsg);
            }

            try
            {
                newStreetcode.Tags.Clear();

                newStreetcode = await _repositoryWrapper.StreetcodeRepository.CreateAsync(newStreetcode);

                _repositoryWrapper?.SaveChangesAsync(cancellationToken);

                var tagIds = request.newStreetcodeContent.Tags.Select(t => t.Id).ToList();

                newStreetcode.Tags.AddRange(await _repositoryWrapper!
                    .TagRepository
                    .GetAllAsync(t => tagIds.Contains(t.Id)));

                _repositoryWrapper?.SaveChangesAsync(cancellationToken);

                return Result.Ok(_mapper.Map<StreetcodeDTO>(newStreetcode));
            }
            catch (Exception ex)
            {
                _logger.LogError(request, ex.Message);
                return Result.Fail(new Error(ex.Message));
            }
        }
    }
}
