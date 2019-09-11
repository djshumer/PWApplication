using System;
using PWApplication.MobileShared.ViewModels;
using PWApplication.MobileShared.ViewModels.Base;

namespace PWApplication.MobileShared.Services.DialogService
{
    public interface IDialogService
    {
        void ShowErrorUserMessage(BaseViewModel viewModel, String message, String cancelBtnText);
        void ShowInformationUserMessage(BaseViewModel viewModel, String message, String cancelBtnText);
        void ShowInfoUserMessage(BaseViewModel viewModel, String title, String message, String cancelBtnText);
        void ShowInfoUserMessage(BaseViewModel viewModel, InfoUserMessage message);
        void ShowActionUserMessage(BaseViewModel viewModel, String title, String message, String cancelBtnText, String okBtnText, Action<bool> callBack);
        void ShowActionUserMessage(BaseViewModel viewModel, ActionUserMessage message);
    }
}