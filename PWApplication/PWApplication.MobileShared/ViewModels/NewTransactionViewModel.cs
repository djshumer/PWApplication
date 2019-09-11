using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Input;
using PWApplication.MobileShared.Models;
using PWApplication.MobileShared.Models.User;
using PWApplication.MobileShared.Services.RequestProvider;
using PWApplication.MobileShared.Services.Settings;
using PWApplication.MobileShared.Services.Transactions;
using PWApplication.MobileShared.Services.UserInfo;
using PWApplication.MobileShared.Services.Users;
using PWApplication.MobileShared.Validations;
using PWApplication.MobileShared.ViewModels.Base;
using Xamarin.Forms;

namespace PWApplication.MobileShared.ViewModels
{
    public class NewTransactionViewModel : BaseViewModel
    {
        private readonly ITransactionService _transactionsService;
        private readonly IUserInfoService _userInfoService;
        private readonly ISettingsService _settingsService;

        private decimal _userBalance = 0;
        private ValidatableObject<decimal> _transactionAmount = new ValidatableObject<decimal>();
        private ValidatableObject<UserInfoSimple> _counteragentView = new ValidatableObject<UserInfoSimple>();
        private string _description = "";

        public NewTransactionViewModel(ISettingsService settingsService, ITransactionService transactionsService, IUserInfoService userInfoService)
        {
            _transactionsService = transactionsService;
            _settingsService = settingsService;
            _userInfoService = userInfoService;

            AddValidations();
        }

        public ICommand CreateTransactionCommand => new Command(async () => await CreateTransactionAsync());

        public ICommand FindUserCommand => new Command(async () => await FindUserAsync());

        public ICommand ValidateTransactionAmountCommand => new Command(ValidateTransactionAmount);

        public override async Task InitializeAsync(object navigationData)
        {
            IsBusy = true;
            try
            {
                var authToken = _settingsService.AuthAccessToken;

                if (navigationData is Transaction)
                {
                    Transaction transaction = navigationData as Transaction;

                    TransactionAmount.Value = Math.Abs(transaction.TransactionAmount);

                    transaction.Description = transaction.Description;

                    CounteragentView.Value = await _userInfoService.GetUserInfo(authToken, transaction.СounteragentId);
                }
    
                UserBalance = await _transactionsService.GetBalance(authToken);
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

        public decimal UserBalance
        {
            get { return _userBalance; }
            protected set { _userBalance = value; RaisePropertyChanged(() => UserBalance); }
        }

        public ValidatableObject<UserInfoSimple> CounteragentView
        {
            get { return _counteragentView; }
            protected set { _counteragentView = value;  RaisePropertyChanged(() => CounteragentView); }
        }

        public ValidatableObject<decimal> TransactionAmount
        {
            get { return _transactionAmount; }
            set { _transactionAmount = value; RaisePropertyChanged(() => TransactionAmount); }
        }

        public string Description
        {
            get { return _description; }
            set { _description = value; RaisePropertyChanged(() => Description); }
        }

        public int CountDescription
        {
            get { return _description != null ? _description.Length : 0; }
        }

        public void SetCounteragent(UserInfoSimple counteragent)
        {
            CounteragentView.Value = counteragent;
        }

        private async Task FindUserAsync()
        {
            IsBusy = true;

            await NavigationService.NavigateToAsync<SearchUserViewModel>();

            IsBusy = false;
        }

        private async Task CreateTransactionAsync()
        {
            if (IsBusy) return;

            if (TransactionAmount.Validate() == false)
                return;

            if (CounteragentView == null)
            {
                DialogService.ShowInformationUserMessage(this, "Counteragent not set", "Cancel");
                return;
            }

            if (TransactionAmount.Value > UserBalance)
            {
                DialogService.ShowInformationUserMessage(this, "Transaction amount greater than user balance.", "Cancel");
                return;
            }

            if (Description == null)
                Description = "";

            IsBusy = true;

            try
            {
                var authToken = _settingsService.AuthAccessToken;
                await _transactionsService.PostTransaction(authToken, CounteragentView.Value.UserId, TransactionAmount.Value, Description);

                DialogService.ShowInformationUserMessage(this, "Transfer Completed", "Ok");

                await NavigationService.NavigationToBackAsync();
            }
            catch (ServiceAuthenticationException)
            {
                await LogoutAsync();
            }
            catch (Exception exc)
            {
                Debug.WriteLine("Error retrieve data: " + exc.Message);
                DialogService.ShowInformationUserMessage(this, "Unable to connect service, check for Internet availability", "Cancel");
            }
            finally
            {
                IsBusy = false;
            }
        }

        private void ValidateTransactionAmount()
        {
            _transactionAmount.Validate();
        }

        private void AddValidations()
        {
            _transactionAmount.Validations.Add(new IsDecimalGreaterThenNull { ValidationMessage = "A Transaction Amount must be greate then 0." });
            _counteragentView.Validations.Add(new IsNotNullOrEmptyRule<UserInfoSimple> { ValidationMessage = "A Counteragent must be set." });
        }

    }
}
