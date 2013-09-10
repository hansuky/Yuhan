using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;

namespace Yuhan.WPF.Extensions
{
    public static class ItemCollectionExtension
    {
        public static void RemoveRange<T>(this ItemCollection collection, IEnumerable<T> items)
        {
            if(items != null){
                for (int index = items.Count() - 1; index >= 0; index--)
                {
                    collection.Remove(items.ElementAt(index));
                }
            }
        }
    }
}
