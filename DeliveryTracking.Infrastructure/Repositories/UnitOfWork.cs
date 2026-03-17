using Delivery_Tracking.Core.Contracts;
using Delivery_Tracking.Core.Entities;
using DeliveryTracking.Infrstructure.Data.Contexts;
using System;
using System.Collections.Generic;
using System.Text;

namespace DeliveryTracking.Infrstructure.Repositories
{
    public class UnitOfWork(DeliveryTrackingDbContext _dbContext) : IUnitOfWork
    {

        private Dictionary<Type, object> _repositories = [];
        public IGenericRepository<TEntity, TKey> GetRepository<TEntity, TKey>() where TEntity : BaseEntity<TKey>
        {
            //Get Type Name
            var type = typeof(TEntity);

            if (_repositories.TryGetValue(type, out var repository))
                return (IGenericRepository<TEntity, TKey>)repository;

            //create repo
            var newRepository = new GenericRepository<TEntity, TKey>(_dbContext);

            // Add to container
            _repositories[type] = newRepository;

            //return the Object
            return newRepository;
        }

        public async Task<int> SaveChangesAsync() => await _dbContext.SaveChangesAsync();
    }
}
