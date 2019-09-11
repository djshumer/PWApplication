using PWApplication.MobileShared.Models;
using PWApplication.MobileShared.ViewModels;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace PWApplication.MobileShared.Extensions
{
    public static class ObservableExtension
    {
        public static ObservableCollection<T> ToObservableCollection<T>(this IEnumerable<T> source)
        {
            ObservableCollection<T> collection = new ObservableCollection<T>();

            foreach (T item in source)
            {
                collection.Add(item);
            }

            return collection;
        }

        public static ObservableCollection<TransactionViewModel> ToTransactionViewModels(IEnumerable<Transaction> source)
        {
            ObservableCollection<TransactionViewModel> newCol = new ObservableCollection<TransactionViewModel>();
            foreach (Transaction tr in source)
            {
                newCol.Add(new TransactionViewModel(tr));
            }
            return newCol;
        }
    }
}
