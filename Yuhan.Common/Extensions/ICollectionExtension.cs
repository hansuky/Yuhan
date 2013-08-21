using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITCFW.Common.Extensions
{
    public static class ICollectionExtension
    {
        public static void AddRange<T>(this ICollection<T> collection, IEnumerable<T> list)
        {
            if(list != null)
                foreach (var item in list)
                    collection.Add(item);
        }
    }
}
