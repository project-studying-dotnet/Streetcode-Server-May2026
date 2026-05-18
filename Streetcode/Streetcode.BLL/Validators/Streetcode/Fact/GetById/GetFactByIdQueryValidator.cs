using Streetcode.BLL.MediatR.Streetcode.Fact.GetById;

namespace Streetcode.BLL.Validators.Streetcode.Fact.GetById
{
    /// <summary>
    /// Validator for GetFactByIdQuery.
    /// </summary>
    public class GetFactByIdQueryValidator : PositiveIdValidator<GetFactByIdQuery>
    {
    }
}