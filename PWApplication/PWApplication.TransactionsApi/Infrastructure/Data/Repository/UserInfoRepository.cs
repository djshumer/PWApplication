using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PWApplication.TransactionApi.Infrastructure.Data.DataModels;

namespace PWApplication.TransactionApi.Infrastructure.Data.Repository
{
    public class UserInfoRepository : GenericRepository<ApplicationUser>, IUserInfoRepository
    {
        public UserInfoRepository(PWTranscationContext transcationContext) : base(transcationContext)
        {

        }

        public async Task<IEnumerable<ApplicationUser>> Find(string userId, string query, int takeCount)
        {
            var lowerQuery = query.ToLower();
            return await dbSet.Where(c => c.Id != userId && (c.UserName.ToLower().Contains(lowerQuery) || c.FullName.ToLower().Contains(lowerQuery)))
                .OrderBy(c => c.UserName)
                .Take(takeCount).ToListAsync();
        }
    }
}
