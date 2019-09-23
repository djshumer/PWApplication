using System.Collections.Generic;
using System.Threading.Tasks;

namespace PWApplication.TransactionApi.Infrastructure.Data.Repository
{
    public interface IGenericRepository<TEntity> where TEntity : class
    {
        Task<TEntity> GetAsync(object id);

        Task<IEnumerable<TEntity>> GetAllAsync();

        void Create(TEntity entity);

        void Delete(params object[] keyValues);

        void Delete(TEntity entity);

        void Update(TEntity entity);
    }
}