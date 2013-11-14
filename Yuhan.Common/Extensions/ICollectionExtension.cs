using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Yuhan.Common.Extensions
{
    public static class ICollectionExtension
    {
        public static void AddRange<T>(this ICollection<T> collection, IEnumerable<T> list)
        {
            if(list != null)
                foreach (var item in list)
                    collection.Add(item);
        }

        public static void RemoveRange<T>(this ICollection<T> collection, IEnumerable<T> list)
        {
            if (list != null)
                for (int index = list.Count() - 1; index >= 0; index--)
                    collection.Remove(list.ElementAt(index));
        }
    }
}
