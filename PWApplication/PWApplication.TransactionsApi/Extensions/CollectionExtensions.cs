using System.Collections.Generic;
using PWApplication.TransactionApi.Infrastructure.Data.DataModels;
using PWApplication.TransactionApi.Models;

namespace PWApplication.TransactionApi.Extensions
{
    public static class CollectionExtensions
    {
        public static IEnumerable<TransactionViewModel> ToTransactionViewModels(this IEnumerable<PWTransaction> col)
        {
            List<TransactionViewModel> newList = new List<TransactionViewModel>();
            foreach (PWTransaction transaction in col)
            {
                newList.Add(new TransactionViewModel(transaction));
            }
            return newList;
        }

        public static IEnumerable<UserInfoViewModel> ToUserInfoViewModels(this IEnumerable<ApplicationUser> col)
        {
            List<UserInfoViewModel> newList = new List<UserInfoViewModel>();
            foreach (ApplicationUser user in col)
            {
                newList.Add(new UserInfoViewModel(user));
            }
            return newList;
        }

        
    }
}
