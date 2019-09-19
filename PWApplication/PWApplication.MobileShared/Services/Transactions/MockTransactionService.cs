using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using PWApplication.MobileShared.Extensions;
using PWApplication.MobileShared.Models;

namespace PWApplication.MobileShared.Services.Transactions
{
    public class MockTransactionService : ITransactionService
    {
        private ObservableCollection<TransactionModel> transactions;

        public MockTransactionService()
        {
            transactions = new ObservableCollection<TransactionModel>();
            for (int i = 0; i < 30; i++)
            {
                transactions.Add(new TransactionModel()
                {
                    Id = Guid.NewGuid(),
                    AgentBalance = 98500 + 50 * i,
                    AgentId = Guid.NewGuid().ToString(),
                    Description = "Bonus",
                    OperationDateTime = DateTime.UtcNow,
                    СounteragentUserName = $"demouser1",
                    TransactionAmount = 50,
                    СounteragentFullName = "John Wick",
                    СounteragentId = Guid.NewGuid().ToString()
                });
            }
        }

        public Task<decimal> GetBalance(string authToken)
        {
            Task.Delay(500);
            return Task.FromResult((decimal)98500.00); 
        }

        public Task<ObservableCollection<TransactionModel>> GetLastTransactions(string authToken, int count)
        {
            Task.Delay(1000);
            return Task.FromResult(transactions.Take(count).ToObservableCollection());
        }

        public Task<TransactionModel> GetTransaction(string authToken, Guid id)
        {
            Task.Delay(500);
            return Task.FromResult(
                new TransactionModel()
                {
                    Id = id,
                    AgentBalance = 98500,
                    AgentId = Guid.NewGuid().ToString(),
                    Description = "Bonus",
                    OperationDateTime = DateTime.UtcNow,
                    СounteragentUserName = $"demouser1",
                    TransactionAmount = 50,
                    СounteragentFullName = "John Wick",
                    СounteragentId = Guid.NewGuid().ToString()
                });
        }

        public Task<ObservableCollection<TransactionModel>> GetTransactionsByDate(string authToken, DateTime startDateTime, DateTime endDateTime)
        {
            Task.Delay(500);
            var list = new ObservableCollection<TransactionModel>();
            for (int i = 0; i < 20; i++)
            {
                list.Add(new TransactionModel()
                {
                    Id = Guid.NewGuid(),
                    AgentBalance = 98500 + 50 * i,
                    AgentId = Guid.NewGuid().ToString(),
                    Description = "Bonus",
                    OperationDateTime = startDateTime.AddSeconds(i),
                    СounteragentUserName = $"demouser1",
                    TransactionAmount = 50,
                    СounteragentFullName = "John Wick",
                    СounteragentId = Guid.NewGuid().ToString()
                });
            }

            return Task.FromResult(list);
        }

        public Task<ObservableCollection<TransactionModel>> GetTransactionsByRange(string authToken, int skip, int take)
        {
            Task.Delay(500);
            var list = new ObservableCollection<TransactionModel>();
            for (int i = 0; i < 20; i++)
            {
                list.Add(new TransactionModel()
                {
                    Id = Guid.NewGuid(),
                    AgentBalance = 98500 + 50 * i,
                    AgentId = Guid.NewGuid().ToString(),
                    Description = "Bonus",
                    OperationDateTime = DateTime.UtcNow,
                    СounteragentUserName = $"demouser1",
                    TransactionAmount = 50,
                    СounteragentFullName = "John Wick",
                    СounteragentId = Guid.NewGuid().ToString()
                });
            }

            return Task.FromResult(list);
        }

        public Task<TransactionModel> PostTransaction(string authToken, string counteragentId, decimal transactionAmount, string description)
        {
            Task.Delay(500);
            TransactionModel transaction = new TransactionModel()
            {
                Id = Guid.NewGuid(),
                AgentBalance = 98500,
                AgentId = Guid.NewGuid().ToString(),
                Description = description,
                OperationDateTime = DateTime.UtcNow,
                СounteragentUserName = $"demouser1",
                TransactionAmount = transactionAmount,
                СounteragentFullName = "John Wick",
                СounteragentId = counteragentId
            };
            transactions.Add(transaction);
            return Task.FromResult(transaction);
        }
    }
}
