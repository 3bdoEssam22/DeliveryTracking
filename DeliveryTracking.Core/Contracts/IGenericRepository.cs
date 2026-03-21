using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace DeliveryTracking.Core.Contracts
{
    public interface IGenericRepository<TEntity, TKey>
    {
        Task<IEnumerable<TEntity>> GetAllAsync();
        Task<IEnumerable<TEntity>> GetAllAsync(
            List<Expression<Func<TEntity, object>>>? includes = null
        );
        Task<IEnumerable<TEntity>> GetAllWhereAsync(
            Expression<Func<TEntity, bool>> criteria,
            List<Expression<Func<TEntity, object>>>? includes = null
        );

        Task<TEntity?> GetByIdAsync(TKey id);
        Task<TEntity?> GetByIdAsync(TKey id,
            List<Expression<Func<TEntity, object>>>? includes = null
        );
        Task AddAsync(TEntity entity);
        void Update(TEntity entity);
        void Delete(TEntity entity);

    }
}
