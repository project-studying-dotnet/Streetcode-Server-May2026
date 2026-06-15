using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Query;

namespace Streetcode.DAL.Specifications.Base;

public interface ISpecification<T>
    where T : class
{
    Expression<Func<T, bool>>? Criteria { get; }

    Func<IQueryable<T>, IIncludableQueryable<T, object>>? Includes { get; }

    Expression<Func<T, object>>? OrderBy { get; }

    bool IsDescending { get; }

    int? Take { get; }

    int? Skip { get; }
}