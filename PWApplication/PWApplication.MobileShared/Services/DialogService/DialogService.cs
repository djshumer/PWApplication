using System;
using PWApplication.MobileShared.ViewModels;
using PWApplication.MobileShared.ViewModels.Base;
using Xamarin.Forms;

namespace PWApplication.MobileShared.Services.DialogService
{
    public class DialogService : IDialogService
    {
        #region infoMessage

        public void ShowErrorUserMessage(BaseViewModel viewModel, String message, String cancelBtnText)
        {
            InfoUserMessage messageInfo = new InfoUserMessage() { Title = "Error", MessageText = message, ButtonCancelText = cancelBtnText };
            MessagingCenter.Send<BaseViewModel, InfoUserMessage>(viewModel, MessageConst.InfoUserMessage, messageInfo);
        }

        public void ShowInformationUserMessage(BaseViewModel viewModel, String message, String cancelBtnText)
        {
            InfoUserMessage messageInfo = new InfoUserMessage() { Title = "Information", MessageText = message, ButtonCancelText = cancelBtnText };
            MessagingCenter.Send<BaseViewModel, InfoUserMessage>(viewModel, MessageConst.InfoUserMessage, messageInfo);
        }

        public void ShowInfoUserMessage(BaseViewModel viewModel, String title, String message, String cancelBtnText)
        {
            InfoUserMessage messageInfo = new InfoUserMessage() { Title = title, MessageText = message, ButtonCancelText = cancelBtnText };
            MessagingCenter.Send<BaseViewModel, InfoUserMessage>(viewModel, MessageConst.InfoUserMessage, messageInfo);
        }

        public void ShowInfoUserMessage(BaseViewModel viewModel, InfoUserMessage message)
        {
            MessagingCenter.Send<BaseViewModel, InfoUserMessage>(viewModel, MessageConst.InfoUserMessage, message);
        }
        #endregion

        #region Action User Message

        public void ShowActionUserMessage(BaseViewModel viewModel, String title, String message, String cancelBtnText, String okBtnText, Action<bool> callBack)
        {
            ActionUserMessage actionMessage = new ActionUserMessage() { Title = title, MessageText = message, ButtonCancelText = cancelBtnText, ButtonOkText = okBtnText, CallBack = callBack };
            MessagingCenter.Send<BaseViewModel, ActionUserMessage>(viewModel, MessageConst.ActionUserMessage, actionMessage);
        }

        public void ShowActionUserMessage(BaseViewModel viewModel, ActionUserMessage message)
        {
            MessagingCenter.Send<BaseViewModel, ActionUserMessage>(viewModel, MessageConst.ActionUserMessage, message);
        }

        #endregion
    }
}
