using System;

namespace PWApplication.MobileShared.Services.DialogService
{
    public class ActionUserMessage : InfoUserMessage
    {

        public String ButtonOkText { get; set; }

        public Action<bool> CallBack { get; set; }

    }
    
}
