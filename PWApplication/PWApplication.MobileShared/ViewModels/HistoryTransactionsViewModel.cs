using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Input;
using PWApplication.MobileShared.Helpers;
using PWApplication.MobileShared.Services.RequestProvider;
using PWApplication.MobileShared.Services.Settings;
using PWApplication.MobileShared.Services.Transactions;
using PWApplication.MobileShared.ViewModels.Base;
using PWApplication.MobileShared.Extensions;
using Xamarin.Forms;
using System.Linq;


namespace PWApplication.MobileShared.ViewModels
{
    public class HistoryTransactionsViewModel : BaseViewModel
    {

        private readonly ISettingsService _settingsService;
        private readonly ITransactionService _transactionService;
        private DateTime _startDate;
        private DateTime _endDate;


        private ObservableCollection<Grouping<string, TransactionViewModel>> _transactionsList;

        public HistoryTransactionsViewModel(ISettingsService settingsService, ITransactionService transactionService)
        {
            _settingsService = settingsService;
            _transactionService = transactionService;

#pragma warning disable CS4014 // Так как этот вызов не ожидается, выполнение существующего метода продолжается до тех пор, пока вызов не будет завершен
            this.InitializeAsync(null);
#pragma warning restore CS4014 // Так как этот вызов не ожидается, выполнение существующего метода продолжается до тех пор, пока вызов не будет завершен
        }

        public DateTime StartDate
        {
            get { return _startDate; }
            set
            {
                if (value != null && value < DateTime.Now)
                {
                    _startDate = value; RaisePropertyChanged(() => StartDate);
                }
            }
        }

        public DateTime EndDate
        {
            get { return _endDate; }
            set
            {
                if (value != null)
                {
                    _endDate = value; RaisePropertyChanged(() => EndDate);
                    if (value < StartDate) StartDate = value;
                }
            }
        }

        public override async Task InitializeAsync(object navigationData)
        {
            StartDate = DateTime.Now.Date.AddDays(-1);
            EndDate = DateTime.Now.Date;

            await RefreshAsync();

            await base.InitializeAsync(navigationData);
        }

        public ICommand TransactionDetailCommand => new Command<TransactionViewModel>(async (tr) => await TransactionDetailAsync(tr));

        public ICommand RefreshCommand => new Command(async () => await RefreshAsync());


        public ObservableCollection<Grouping<String, TransactionViewModel>> TransactionsList
        {
            get { return _transactionsList; }
            protected set { _transactionsList = value; RaisePropertyChanged(() => TransactionsList); }
        }

        private async Task TransactionDetailAsync(TransactionViewModel transaction)
        {
            await NavigationService.NavigateToAsync<TransactionDetailViewModel>(transaction.Transaction);
        }

        private async Task RefreshAsync()
        {
            if (IsBusy) return;

            IsBusy = true;

            try
            {
                var authToken = _settingsService.AuthAccessToken;

                var trList = await _transactionService.GetTransactionsByDate(authToken, StartDate.ToUniversalTime(), EndDate.ToUniversalTime());
                var trViewModelList = ObservableExtension.ToTransactionViewModels(trList);
                TransactionsList = trViewModelList.OrderByDescending(c => c.OperationDateTime)
                    .GroupBy(c => c.OperationDateView)
                    .Select(c => new Grouping<string, TransactionViewModel>(c.Key, c))
                    .ToObservableCollection();

            }
            catch (ServiceAuthenticationException ex)
            {
                Debug.WriteLine($"[RetrieveData] Error Retrieve Data: {ex}");
                await LogoutAsync();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[RetrieveData] Error Retrieve Data: {ex}");
                DialogService.ShowInformationUserMessage(this, "unable to retrieve data, check for Internet availability", "Cancel");
            }
            finally
            {
                IsBusy = false;
            }

        }

    }
}
