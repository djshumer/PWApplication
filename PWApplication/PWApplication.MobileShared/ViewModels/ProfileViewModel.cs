using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using PWApplication.MobileShared.Extensions;
using PWApplication.MobileShared.Helpers;
using PWApplication.MobileShared.Models;
using PWApplication.MobileShared.Models.User;
using PWApplication.MobileShared.Services.RequestProvider;
using PWApplication.MobileShared.Services.Settings;
using PWApplication.MobileShared.Services.Transactions;
using PWApplication.MobileShared.Services.UserInfo;
using PWApplication.MobileShared.ViewModels.Base;
using Xamarin.Forms;

namespace PWApplication.MobileShared.ViewModels
{
    public class ProfileViewModel : BaseViewModel
    {
        private readonly ISettingsService _settingsService;
        private readonly IUserInfoService _userInfoService;
        private readonly ITransactionService _transactionService;

        private ObservableCollection<Grouping<string, TransactionViewModel>> _transactionsList = new ObservableCollection<Grouping<string, TransactionViewModel>>();
        private AppUserInfo _userInfo;
        private decimal _balance = 0;

       
        public ProfileViewModel(ISettingsService settingsService, IUserInfoService userInfoService, ITransactionService transactionService) : base()
        {
            _settingsService = settingsService;
            _userInfoService = userInfoService;
            _transactionService = transactionService;
        }

        public ObservableCollection<Grouping<string, TransactionViewModel>> TransactionsList
        {
            get { return _transactionsList; }
            protected set { _transactionsList = value; RaisePropertyChanged(() => TransactionsList); }
        }

        public AppUserInfo UserInfo
        {
            get { return _userInfo; }
            protected set { _userInfo = value; RaisePropertyChanged(() => UserInfo); }
        }

        public decimal UserBalance
        {
            get { return _balance; }
            protected set { _balance = value; RaisePropertyChanged(() => UserBalance); }
        }

        public ICommand TransactionDetailCommand => new Command<TransactionViewModel>(async (tr) => await TransactionDetailAsync(tr));

        public ICommand RefreshCommand => new Command(async () => await RefreshAsync());

        public ICommand TransferCommand => new Command(async () => await NewTransferAsync());

        public override async Task InitializeAsync(object navigationData)
        {
            await RefreshAsync();
        }

        private async Task RefreshAsync()
        {
            if (IsBusy) return;

            IsBusy = true;

            try
            {
                var authToken = _settingsService.AuthAccessToken;

                UserInfo = await _userInfoService.GetCurrentUserInfoAsync(authToken);

                var trList = await _transactionService.GetLastTransactions(authToken, 10);
                var trViewModelList = ToTransactionViewModels(trList);
                TransactionsList = trViewModelList.OrderByDescending(c => c.OperationDateTime)
                    .GroupBy(c => c.OperationDateView)
                    .Select(c => new Grouping<string, TransactionViewModel>(c.Key, c))
                    .ToObservableCollection();

                UserBalance = UpdateUserBalanceByTranList(trList);
            }
            catch (ServiceAuthenticationException)
            {
                await LogoutAsync();
            }
            catch (Exception exc)
            {
                Debug.WriteLine("Error retrieve data: " + exc.Message);
                DialogService.ShowInformationUserMessage(this, "unable to retrieve data, check for Internet availability", "Cancel");
            }
            finally
            {
                IsBusy = false;
            }

        }

        private async Task NewTransferAsync()
        {
            await NavigationService.NavigateToAsync<NewTransactionViewModel>();
        }

        private decimal UpdateUserBalanceByTranList(IList<TransactionModel> transactions)
        {
            if (transactions.Any())
            {
                return transactions.OrderByDescending(transaction => transaction.OperationDateTime)
                    .Select(c => c.AgentBalance).FirstOrDefault();
            }

            return 0;
        }


        private async Task TransactionDetailAsync(TransactionViewModel transaction)
        {
            await NavigationService.NavigateToAsync<TransactionDetailViewModel>(transaction.Transaction);
        }

        private ObservableCollection<TransactionViewModel> ToTransactionViewModels(IEnumerable<TransactionModel> source)
        {
            ObservableCollection<TransactionViewModel> newCol = new ObservableCollection<TransactionViewModel>();
            foreach (TransactionModel tr in source)
            {
                newCol.Add(new TransactionViewModel(tr));
            }
            return newCol;
        }
    }
}
