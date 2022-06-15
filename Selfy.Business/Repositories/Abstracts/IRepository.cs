using Selfy.Core.Entities.Abstracts;
using System.Linq.Expressions;

namespace Selfy.Business.Repositories.Abstracts
{
    public interface IRepository<TEntity, TKey>
        where TKey : IEquatable<TKey>
        where TEntity : BaseEntity<TKey>
    {
        IQueryable<TEntity> Get(Expression
            <Func<TEntity, bool>> predicate = null);

        TEntity GetById(TKey id);

        TKey Insert(TEntity entity, bool isSaveLater = false);

        int Update( TEntity entity, bool isSaveLater = false);

        int Delete(TEntity entity, bool isSaveLater = false);

        int Save();
    }
}
