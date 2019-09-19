using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using PWApplication.MobileShared.Models;

namespace PWApplication.MobileShared.Services.Transactions
{
    public interface ITransactionService
    {
        Task<ObservableCollection<TransactionModel>> GetLastTransactions(string authToken, int count);
        Task<ObservableCollection<TransactionModel>> GetTransactionsByRange(string authToken, int skip, int take);
        Task<ObservableCollection<TransactionModel>> GetTransactionsByDate(string authToken, DateTime startDateTime, DateTime endDateTime);
        Task<decimal> GetBalance(string authToken);
        Task<TransactionModel> GetTransaction(string authToken, Guid id);
        Task<TransactionModel> PostTransaction(string authToken, string counteragentId, decimal transactionAmount, string description);
    }
}