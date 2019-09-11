using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using PWApplication.MobileShared.Models;

namespace PWApplication.MobileShared.Services.Transactions
{
    public interface ITransactionService
    {
        Task<ObservableCollection<Transaction>> GetLastTransactions(string authToken, int count);
        Task<ObservableCollection<Transaction>> GetTransactionsByRange(string authToken, int skip, int take);
        Task<ObservableCollection<Transaction>> GetTransactionsByDate(string authToken, DateTime startDateTime, DateTime endDateTime);
        Task<decimal> GetBalance(string authToken);
        Task<Transaction> GetTransaction(string authToken, Guid id);
        Task<Transaction> PostTransaction(string authToken, string counteragentId, decimal transactionAmount, string description);
    }
}