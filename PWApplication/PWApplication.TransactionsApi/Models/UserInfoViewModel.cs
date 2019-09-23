using PWApplication.TransactionApi.Infrastructure.Data.DataModels;

namespace PWApplication.TransactionApi.Models
{
    public class UserInfoViewModel
    {

        public UserInfoViewModel(ApplicationUser applicationUser)
        {
            UserId = applicationUser.Id;
            Email = applicationUser.Email;
            UserName = applicationUser.UserName;
            FullName = applicationUser.FullName;
        }

        public string UserId { get; set; }

        public string FullName { get; set; }

        public string UserName { get; set; }

        public string Email { get; set; }

    }
}
