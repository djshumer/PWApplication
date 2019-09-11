using PWApplication.MobileShared.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace PWApplication.MobileShared.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class NewTransactionView : ContentPage
    {
        public NewTransactionView()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            //var viewModel = BindingContext as NewTransactionViewModel;
            //if(viewModel != null) viewModel.RefreshView();
        }

    }
}