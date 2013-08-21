using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Yuhan.Common.Extensions
{
    public static class IntExtension
    {
        public static String AsCurrency(this int value)
        {
            return value.AsCurrency(CultureInfo.CurrentCulture);
        }

        public static string AsCurrency(this int value, CultureInfo culture)
        {
            return value.ToString("c", culture);
        }
    }
}
