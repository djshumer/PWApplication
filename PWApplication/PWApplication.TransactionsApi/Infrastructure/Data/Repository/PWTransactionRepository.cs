using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PWApplication.TransactionApi.Infrastructure.Data.DataModels;

namespace PWApplication.TransactionApi.Infrastructure.Data.Repository
{
    public class PWTransactionRepository : GenericRepository<PWTransaction>, IPWTransactionRepository
    {

        public PWTransactionRepository(PWTranscationContext context) : base(context)
        {

        }

        public async Task<IEnumerable<PWTransaction>> GetTransactionsByRangeAsync(string userId, int skip = 0, int count = 0)
        {
            return await dbSet.Where(c => c.AgentId == userId)
                         .Include(c => c.Сounteragent)
                         .OrderByDescending(c => c.OperationDateTime)
                         .Skip(skip).Take(count)
                         .ToListAsync();
        }

        public async Task<IEnumerable<PWTransaction>> GetTransactionsByDateAsync(string userId, DateTime startDate, DateTime endDate)
        {
            return await dbSet.Where(c => c.AgentId == userId && c.OperationDateTime >= startDate && c.OperationDateTime <= endDate)
                .Include(c => c.Сounteragent)
                .OrderByDescending(c => c.OperationDateTime)
                .ToListAsync();
        }

        public async Task<IEnumerable<PWTransaction>> GetLastTransactionsWithIncludeAsync(string userId, int count)
        {
            return await dbSet.Where(c => c.AgentId == userId)
                .OrderByDescending(c => c.OperationDateTime)
                .Include(c => c.Сounteragent)
                .Take(count)
                .ToListAsync();
        }

        public async Task<PWTransaction> GetLastTransaction(string userId)
        {
            var lastTransaction = await dbSet
                .Where(c => c.AgentId == userId)
                .OrderByDescending(c => c.OperationDateTime)
                .FirstOrDefaultAsync();
            return lastTransaction;
        }

        public async Task<PWTransaction> GetTransactionWithInclude(string currentUserId, Guid transactionId)
        {
            return await dbSet.Where(c => c.AgentId == currentUserId)
                .Include(c => c.Сounteragent)
                .FirstOrDefaultAsync(c => c.Id == transactionId);
        }
    }
}
