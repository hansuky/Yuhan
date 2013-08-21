using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace Yuhan.WPF.Converters
{
    public class DecimalToQuantityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            Decimal val = (Decimal)value;
            return val.ToString("N");
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return Decimal.Parse(value.ToString().Replace(',', '\0'));
        }
    }
}
