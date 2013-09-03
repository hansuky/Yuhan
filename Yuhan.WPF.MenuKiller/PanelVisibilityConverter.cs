using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Yuhan.WPF.MenuKiller
{
    [ValueConversion(typeof(bool), typeof(Visibility))]
    public class PanelVisibilityConverter : IValueConverter
    {
        public object Convert(
            object value,
            Type targetType,
            object param,
            CultureInfo culture)
        {
            bool bIsEnabled = (bool)value;

            if (bIsEnabled)
                return Visibility.Visible;
            else
                return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Note: Of course, this conversion is possible; it is even 'better' than the 
            // convert method, because it is well-defined while the above is debateable. 
            // We don't need it, though.
            throw new NotSupportedException("Converter used in wrong direction!");
        }
    }
}
