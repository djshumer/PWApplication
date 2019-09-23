using System.Collections.Generic;
using System.Threading.Tasks;
using PWApplication.TransactionApi.Infrastructure.Data.DataModels;

namespace PWApplication.TransactionApi.Infrastructure.Data.Repository
{
    public interface IUserInfoRepository : IGenericRepository<ApplicationUser>
    {
        Task<IEnumerable<ApplicationUser>> Find(string userId, string query, int takeCount);
    }
}