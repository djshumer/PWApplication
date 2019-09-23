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
        private AppUserInfo _currentUserInfo;
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

                CurrentUser = await _userInfoService.GetCurrentUserInfoAsync(authToken);

                ReplaceCounteragentUserValidation(CurrentUser);

                UserBalance = await _transactionsService.GetBalance(authToken);

                if (navigationData is TransactionModel)
                {
                    TransactionModel transaction = navigationData as TransactionModel;

                    TransactionAmount.Value = Math.Abs(transaction.TransactionAmount);

                    transaction.Description = transaction.Description;

                    CounteragentView.Value = await _userInfoService.GetUserInfoAsync(authToken, transaction.СounteragentId);
                }
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

        public AppUserInfo CurrentUser
        {
            get { return _currentUserInfo; }
            protected set { _currentUserInfo = value; RaisePropertyChanged(() => CurrentUser); }
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
            catch (ServiceAuthenticationException ex) 
            {
                Debug.WriteLine($"[RetrieveData] Error Retrieve Data: {ex}");
                await LogoutAsync();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[RetrieveData] Error Retrieve Data: {ex}");
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

        private void ReplaceCounteragentUserValidation(AppUserInfo currentUser)
        {
            var findRule = _counteragentView.Validations.Find(s => s.GetType() == typeof(IsNotEqualCurrentUser));

            if (findRule != null)
            _counteragentView.Validations.Remove(findRule);
            //This situation is unlikely because the server does not return the user on whose behalf the request was made. Manual data entry is blocked.
            _counteragentView.Validations.Add(new IsNotEqualCurrentUser(currentUser) {ValidationMessage = "A Counteragent must be not equal current User" });
        }

        private void AddValidations()
        {
            _transactionAmount.Validations.Add(new IsDecimalGreaterThenNull() { ValidationMessage = "A Transaction Amount must be greate then 0." });
            _counteragentView.Validations.Add(new IsNotNullOrEmptyRule<UserInfoSimple>() { ValidationMessage = "A Counteragent must be set." });
        }

    }
}
