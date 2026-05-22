using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Query;

namespace Streetcode.DAL.Specifications.Base;

public abstract class BaseSpecification<T> : ISpecification<T>
    where T : class
{
    protected BaseSpecification()
    {
    }

    protected BaseSpecification(Expression<Func<T, bool>> criteria)
    {
        Criteria = criteria;
    }

    public Expression<Func<T, bool>>? Criteria { get; protected set; }

    public Func<IQueryable<T>, IIncludableQueryable<T, object>>? Includes { get; protected set; }

    public Expression<Func<T, object>>? OrderBy { get; protected set; }

    public bool IsDescending { get; protected set; }

    public int? Take { get; protected set; }

    public int? Skip { get; protected set; }
}