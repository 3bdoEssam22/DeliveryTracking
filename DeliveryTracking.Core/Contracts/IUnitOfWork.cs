using DeliveryTracking.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace DeliveryTracking.Core.Contracts
{
    public interface IUnitOfWork : IDisposable, IAsyncDisposable
    {
        IGenericRepository<TEntity, TKey> GetRepository<TEntity, TKey>() where TEntity : BaseEntity<TKey>;

        Task<int> SaveChangesAsync();
    }
}
