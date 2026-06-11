using System;
using System.Linq;
using System.Linq.Expressions;
using Streetcode.DAL.Entities.Streetcode;
using Streetcode.DAL.Repositories.Interfaces.Base;

namespace Streetcode.DAL.Repositories.Interfaces.Streetcode
{
    public interface IRelatedFigureRepository : IRepositoryBase<RelatedFigure>
    {
        IQueryable<RelatedFigure> FindAll(Expression<Func<RelatedFigure, bool>> expression);
    }
}