using DeliveryTracking.Core.Contracts;
using DeliveryTracking.Core.Entities;
using DeliveryTracking.Infrastructure.Data.Contexts;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace DeliveryTracking.Infrastructure.Repositories
{
    public class GenericRepository<TEntity, TKey>(DeliveryTrackingDbContext DbContext) : IGenericRepository<TEntity, TKey> where TEntity : BaseEntity<TKey>
    {
        public async Task AddAsync(TEntity entity) => await DbContext.Set<TEntity>().AddAsync(entity);
        public void Delete(TEntity entity) => DbContext.Set<TEntity>().Remove(entity);
        public void Update(TEntity entity) => DbContext.Set<TEntity>().Update(entity);


        public async Task<IEnumerable<TEntity>> GetAllAsync() => await DbContext.Set<TEntity>().ToListAsync();


        public async Task<TEntity?> GetByIdAsync(TKey id) => await DbContext.Set<TEntity>().FindAsync(id);

        public async Task<IEnumerable<TEntity>> GetAllAsync(List<Expression<Func<TEntity, object>>>? includes = null)
        {
            var query = DbContext.Set<TEntity>().AsQueryable();
            if (includes != null)
            {
                foreach (var include in includes)
                {
                    query = query.Include(include);
                }
            }
            return await query.ToListAsync();
        }

        public async Task<TEntity?> GetByIdAsync(TKey id, List<Expression<Func<TEntity, object>>>? includes = null)
        {
            var query = DbContext.Set<TEntity>().AsQueryable();
            if (includes != null)
            {
                foreach (var include in includes)
                {
                    query = query.Include(include);
                }
            }
            return await query.FirstOrDefaultAsync(e => e.Id!.Equals(id));
        }
    }
}
