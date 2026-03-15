using Delivery_Tracking.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Delivery_Tracking.Core.Contracts
{
    public interface IUnitOfWork
    {
        IGenericRepository<TEntity, TKey> GetRepository<TEntity, TKey>() where TEntity : BaseEntity<TKey>;

        Task<int> SaveChangesAsync();

    }
}
