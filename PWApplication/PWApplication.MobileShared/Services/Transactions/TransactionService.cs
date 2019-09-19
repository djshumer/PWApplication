using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using PWApplication.MobileShared.Extensions;
using PWApplication.MobileShared.Helpers;
using PWApplication.MobileShared.Models;
using PWApplication.MobileShared.Services.RequestProvider;

namespace PWApplication.MobileShared.Services.Transactions
{
    public class TransactionService : ITransactionService
    {
        private readonly IRequestProvider _requestProvider;
        
        private const string ApiUrlBase = "api/v1/transactions";

        public TransactionService(IRequestProvider requestProvider)
        {
            _requestProvider = requestProvider;
        }

        // GET api/v1/transactions/last[?count=10]
        public async Task<ObservableCollection<TransactionModel>> GetLastTransactions(string authToken, int count)
        {
            var uri = UriHelper.CombineUri(GlobalSetting.Instance.GatewayPWEndpoint, $"{ApiUrlBase}/last?count={count}");

            var transactionsList = await _requestProvider.GetAsync<IEnumerable<TransactionModel>>(uri, authToken);

            if (transactionsList != null)
                return transactionsList.ToObservableCollection();
            else
                return new ObservableCollection<TransactionModel>();
        }

        // GET api/v1/transactions/range[?skip=100&count=100]
        public async Task<ObservableCollection<TransactionModel>> GetTransactionsByRange(string authToken, int skip, int count)
        {
            var uri = UriHelper.CombineUri(GlobalSetting.Instance.GatewayPWEndpoint, $"{ApiUrlBase}/range?skip={skip}&count={count}");

            IEnumerable<TransactionModel> transactionsList = await _requestProvider.GetAsync<IEnumerable<TransactionModel>>(uri, authToken);

            if (transactionsList != null)
                return transactionsList.ToObservableCollection();
            else
                return new ObservableCollection<TransactionModel>();
        }

        // GET api/v1/transactions/bydate[?startDate=10.10.2019&endDate=15.10.2019]
        public async Task<ObservableCollection<TransactionModel>> GetTransactionsByDate(string authToken, DateTime startDateTime, DateTime endDateTime)
        {
            var uri = UriHelper.CombineUri(GlobalSetting.Instance.GatewayPWEndpoint, $"{ApiUrlBase}/bydate?startDate={startDateTime}&endDate={endDateTime}");

            IEnumerable<TransactionModel> transactionsList = await _requestProvider.GetAsync<IEnumerable<TransactionModel>>(uri, authToken);

            if (transactionsList != null)
                return transactionsList.ToObservableCollection();
            else
                return new ObservableCollection<TransactionModel>();
        }

        // GET api/v1/transactions/balance
        public async Task<decimal> GetBalance(string authToken)
        {
            var uri = UriHelper.CombineUri(GlobalSetting.Instance.GatewayPWEndpoint, $"{ApiUrlBase}/balance");

            decimal? balance = await _requestProvider.GetAsync<decimal>(uri, authToken);

            if (balance != null)
                return balance.Value;
            else
                return 0;
        }

        // GET: api/transactions/5
        public async Task<TransactionModel> GetTransaction(string authToken, Guid id)
        {
            var uri = UriHelper.CombineUri(GlobalSetting.Instance.GatewayPWEndpoint, $"{ApiUrlBase}/{id}");

            TransactionModel transaction = await _requestProvider.GetAsync<TransactionModel>(uri, authToken);

            return transaction;
        }

        // POST: api/transactions/transfer
        public async Task<TransactionModel> PostTransaction(string authToken, string counteragentId, decimal transactionAmount, string description)
        {
            var uri = UriHelper.CombineUri(GlobalSetting.Instance.GatewayPWEndpoint, $"{ApiUrlBase}/transfer");

            NewTransactionModel newTransactionModel = new NewTransactionModel()
            {
                CounteragentId = counteragentId,
                TransactionAmount = transactionAmount,
                Description = description
            };

            TransactionModel transaction = await _requestProvider.PostAsync<TransactionModel, NewTransactionModel>(uri, newTransactionModel, authToken);

            return transaction;
        }
    }
}
