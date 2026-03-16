using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Delivery_Tracking.Core.Contracts
{
    public interface IGenericRepository<TEntity, TKey>
    {
        Task<IEnumerable<TEntity>> GetAllAsync();
        Task<TEntity?> GetByIdAsync(TKey id);
        Task AddAsync(TEntity entity);
        void Update(TEntity entity);
        void Delete(TEntity entity);

    }
}
