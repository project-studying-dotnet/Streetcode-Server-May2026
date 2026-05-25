using FluentResults;
using MediatR;
using Streetcode.DAL.Repositories.Interfaces.Base;
using Streetcode.BLL.Resources;

namespace Streetcode.BLL.MediatR.AdditionalContent.Coordinate.Delete;

public class DeleteCoordinateHandler : IRequestHandler<DeleteCoordinateCommand, Result<Unit>>
{
    private readonly IRepositoryWrapper _repositoryWrapper;

    public DeleteCoordinateHandler(IRepositoryWrapper repositoryWrapper)
    {
        _repositoryWrapper = repositoryWrapper;
    }

    public async Task<Result<Unit>> Handle(DeleteCoordinateCommand request, CancellationToken cancellationToken)
    {
        var streetcodeCoordinate = await _repositoryWrapper.StreetcodeCoordinateRepository
            .GetFirstOrDefaultAsync(
                predicate: f => f.Id == request.Id,
                cancellationToken: cancellationToken);

        if (streetcodeCoordinate is null)
        {
            return Result.Fail(new Error(string.Format(ErrorMessages.CannotFindCoordinateByCategoryId, request.Id)));
        }

        _repositoryWrapper.StreetcodeCoordinateRepository.Delete(streetcodeCoordinate);

        var resultIsSuccess = await _repositoryWrapper.SaveChangesAsync(cancellationToken) > 0;
        return resultIsSuccess ? Result.Ok(Unit.Value) : Result.Fail(new Error(ErrorMessages.FailedToDeleteCoordinate));
    }
}