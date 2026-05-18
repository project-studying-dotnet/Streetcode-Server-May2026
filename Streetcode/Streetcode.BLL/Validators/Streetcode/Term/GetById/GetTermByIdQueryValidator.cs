using Streetcode.BLL.MediatR.Streetcode.Term.GetById;

namespace Streetcode.BLL.Validators.Streetcode.Term.GetById
{
    /// <summary>
    /// Validator for <see cref="GetTermByIdQuery"/>.
    /// </summary>
    public class GetTermByIdQueryValidator : PositiveIdValidator<GetTermByIdQuery>
    {
    }
}
