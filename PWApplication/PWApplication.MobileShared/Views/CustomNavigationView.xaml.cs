using System.Threading.Tasks;
using PWApplication.MobileShared.Services.DialogService;
using PWApplication.MobileShared.ViewModels;
using PWApplication.MobileShared.ViewModels.Base;
using Xamarin.Forms;

namespace PWApplication.MobileShared.Views
{
    public partial class CustomNavigationView : NavigationPage
    {
        public CustomNavigationView() : base()
        {
            InitializeComponent();

            MessagingCenter.Subscribe<BaseViewModel, InfoUserMessage>(this, MessageConst.InfoUserMessage, async (v, m) => await ShowInfoUserMessageAsync(v, m));
            MessagingCenter.Subscribe<BaseViewModel, ActionUserMessage>(this, MessageConst.ActionUserMessage, async (v, m) => await ShowActionUserMessageAsync(v, m));
        }

        public CustomNavigationView(Page root) : base(root)
        {
            InitializeComponent();

            MessagingCenter.Subscribe<BaseViewModel, InfoUserMessage>(this, MessageConst.InfoUserMessage, async (v, m) => await ShowInfoUserMessageAsync(v, m));
            MessagingCenter.Subscribe<BaseViewModel, ActionUserMessage>(this, MessageConst.ActionUserMessage, async (v, m) => await ShowActionUserMessageAsync(v, m));
        }

        protected async Task ShowInfoUserMessageAsync(BaseViewModel viewModel, InfoUserMessage userMessage)
        {
            if (userMessage == null)
                return;

            await DisplayAlert(userMessage.Title, userMessage.MessageText, userMessage.ButtonCancelText);
        }

        protected async Task ShowActionUserMessageAsync(BaseViewModel viewModel, ActionUserMessage userMessage)
        {
            if (userMessage == null)
                return;

            bool result = await DisplayAlert(userMessage.Title, userMessage.MessageText, userMessage.ButtonOkText, userMessage.ButtonCancelText);

            if (userMessage.CallBack != null)
            {
                userMessage.CallBack.Invoke(result);
            }
        }
    }
}