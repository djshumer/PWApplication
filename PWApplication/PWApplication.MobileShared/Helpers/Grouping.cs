using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace PWApplication.MobileShared.Helpers
{
    public class Grouping<K, T> : ObservableCollection<T>
    {
        public K Key { get; private set; }
        public Grouping(K key, IEnumerable<T> items)
        {
            Key = key;
            foreach (T item in items)
                Items.Add(item);
        }
    }
}
