using PWApplication.MobileAppService.Models.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PWApplication.MobileAppService.Models
{
    public class UserInfoViewModel
    {

        public UserInfoViewModel(ApplicationUser applicationUser)
        {
            Id = applicationUser.Id;
            Email = applicationUser.Email;
            UserName = applicationUser.UserName;
            FullName = applicationUser.FullName;
        }

        public string Id { get; set; }

        public string FullName { get; set; }

        public string UserName { get; set; }

        public string Email { get; set; }

    }
}
