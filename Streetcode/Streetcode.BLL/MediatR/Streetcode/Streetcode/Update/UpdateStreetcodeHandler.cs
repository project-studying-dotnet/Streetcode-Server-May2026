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
using Streetcode.DAL.Entities.AdditionalContent;

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
                    .GetFirstOrDefaultAsync(s => s.Id == request.streetcode.Id);

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

            try
            {
                streetcode.Tags.Clear();

                _repositoryWrapper.StreetcodeRepository.Update(streetcode);

                _repositoryWrapper?.SaveChangesAsync();

                var newTagIds = request.streetcode.Tags.Select(t => t.Id).ToList();

                var oldTags = await _repositoryWrapper.StreetcodeTagIndexRepository
                    .GetAllAsync(t => t.StreetcodeId == streetcode.Id);

                foreach (var oldTag in oldTags)
                {
                    if (!newTagIds.Contains(oldTag.TagId))
                    {
                        _repositoryWrapper.StreetcodeTagIndexRepository.Delete(oldTag);
                    }
                }

                foreach (var newTagId in newTagIds)
                {
                    if (oldTags.FirstOrDefault(t => t.TagId == newTagId) == null)
                    {
                        _repositoryWrapper?.StreetcodeTagIndexRepository.CreateAsync(new StreetcodeTagIndex
                        {
                            StreetcodeId = streetcode.Id,
                            TagId = newTagId
                        });
                    }
                }

                _repositoryWrapper?.SaveChangesAsync();

                var response = _mapper.Map<StreetcodeDTO>(streetcode);
                response.Tags = request.streetcode.Tags;
                return Result.Ok(response);
            }
            catch(Exception ex)
            {
                _logger.LogError(request, ex.Message);
                return Result.Fail(new Error(ex.Message));
            }
        }
    }
}
