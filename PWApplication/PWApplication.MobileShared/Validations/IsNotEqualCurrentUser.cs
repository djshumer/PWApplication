using PWApplication.MobileShared.Models.User;
using System;
using System.Collections.Generic;
using System.Text;

namespace PWApplication.MobileShared.Validations
{
    public class IsNotEqualCurrentUser : IValidationRule<UserInfoSimple>
    {
        private AppUserInfo _currentUser;

        public IsNotEqualCurrentUser(AppUserInfo currentUser)
        {
            _currentUser = currentUser;
        }

        public string ValidationMessage { get; set; }

        public bool Check(UserInfoSimple value)
        {
            if (value == null || _currentUser.UserId != value.UserId)
            {
                return true;
            }

            return false;
        }
    }

}
