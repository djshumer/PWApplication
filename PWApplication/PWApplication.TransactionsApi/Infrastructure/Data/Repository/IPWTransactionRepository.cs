using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PWApplication.TransactionApi.Infrastructure.Data.DataModels;

namespace PWApplication.TransactionApi.Infrastructure.Data.Repository
{
    public interface IPWTransactionRepository : IGenericRepository<PWTransaction>
    {
        Task<IEnumerable<PWTransaction>> GetLastTransactionsWithIncludeAsync(string userId, int count);

        Task<IEnumerable<PWTransaction>> GetTransactionsByDateAsync(string userId, DateTime startDate, DateTime endDate);

        Task<IEnumerable<PWTransaction>> GetTransactionsByRangeAsync(string userId, int skip = 0, int count = 0);

        Task<PWTransaction> GetTransactionWithInclude(string currentUserId, Guid transactionId);

        Task<PWTransaction> GetLastTransaction(string userId);
    }
}