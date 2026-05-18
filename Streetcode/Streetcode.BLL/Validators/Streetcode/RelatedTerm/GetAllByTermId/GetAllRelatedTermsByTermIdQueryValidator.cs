using Streetcode.BLL.MediatR.Streetcode.RelatedTerm.GetAllByTermId;

namespace Streetcode.BLL.Validators.Streetcode.RelatedTerm.GetAllByTermId
{
    /// <summary>
    /// Validator for GetAllRelatedTermsByTermIdQuery.
    /// </summary>
    public class GetAllRelatedTermsByTermIdQueryValidator
        : PositiveIdValidator<GetAllRelatedTermsByTermIdQuery>
    {
    }
}