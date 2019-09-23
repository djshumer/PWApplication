using Microsoft.AspNetCore.Identity;

namespace PWApplication.TransactionApi.Infrastructure.Data.DataModels
{
    public class ApplicationUser : IdentityUser
    {       
        public string FullName { get; set; }
    }
}
