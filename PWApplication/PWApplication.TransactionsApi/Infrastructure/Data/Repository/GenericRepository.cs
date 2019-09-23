using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace PWApplication.TransactionApi.Infrastructure.Data.Repository
{
    public class GenericRepository<TEntity> : IGenericRepository<TEntity> where TEntity : class, new()
    {
        protected PWTranscationContext context;
        protected DbSet<TEntity> dbSet;

        public GenericRepository(PWTranscationContext context)
        {
            this.context = context;
            this.dbSet = context.Set<TEntity>();
        }

        public async Task<IEnumerable<TEntity>> GetAllAsync()
        {
            return await context.Set<TEntity>().ToListAsync();
        }

        public async Task<TEntity> GetAsync(object id)
        {
            return await context.Set<TEntity>().FindAsync(id);
        }

        public void Create(TEntity entity)
        {
            context.Set<TEntity>().Add(entity);
        }

        public void Update(TEntity entity)
        {
            if (entity == null)
                return;
            if (context.Entry(entity).State == EntityState.Detached)
            {
                context.Attach<TEntity>(entity);
            }
            context.Entry(entity).State = EntityState.Modified;
        }

        public void Delete(params object[] keyValues)
        {
            TEntity entity = context.Set<TEntity>().Find(keyValues);
            if (entity != null)
                context.Set<TEntity>().Remove(entity);
        }

        public void Delete(TEntity entity)
        {
            if (entity != null)
            {
                if (context.Entry<TEntity>(entity).State == EntityState.Detached)
                {
                    context.Attach<TEntity>(entity);
                }

                context.Set<TEntity>().Remove(entity);
            }
        }
    }
}
