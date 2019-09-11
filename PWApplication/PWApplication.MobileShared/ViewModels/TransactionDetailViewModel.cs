using System;
using System.Threading.Tasks;
using System.Windows.Input;
using PWApplication.MobileShared.Models;
using PWApplication.MobileShared.ViewModels.Base;
using Xamarin.Forms;

namespace PWApplication.MobileShared.ViewModels
{
    public class TransactionDetailViewModel : BaseViewModel
    {
        private TransactionViewModel _transactionView;
     
        public TransactionDetailViewModel()
        {
        }

        public override async Task InitializeAsync(object navigationData)
        {
            if (navigationData is Transaction)
            {
                var transaction = navigationData as Transaction;
                TransactionView = new TransactionViewModel(transaction);
            }
            else
            {
                throw new ArgumentException("NavigationData in TransactionDetailViewModel is invalid");
            }
        }

        #region Props

        public TransactionViewModel TransactionView
        {
            get { return _transactionView; }
            private set { _transactionView = value;  RaisePropertyChanged(() => TransactionView);}
        }

        #endregion Props 
        public ICommand NewTransactionCommand => new Command(async () => await CreateNewTransactionAsync());
        
        private async Task CreateNewTransactionAsync()
        {
            if(_transactionView == null || _transactionView.Transaction == null)
                return;

            await NavigationService.NavigateToAsync<NewTransactionViewModel>(_transactionView.Transaction);
        }
    }
}
