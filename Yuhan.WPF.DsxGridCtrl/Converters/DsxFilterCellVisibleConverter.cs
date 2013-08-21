using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Globalization;
using System.Windows;
using System.ComponentModel;

namespace Yuhan.WPF.DsxGridCtrl
{
    [ValueConversion(typeof(DsxColumn), typeof(Visibility))]
    public class DsxFilterCellVisibleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (parameter == null || parameter.ToString()==String.Empty)
            {
                return Visibility.Collapsed;
            }

            bool _checkBox = parameter.ToString()=="CheckDisplay";

            if (value != null && value is DsxColumn)
            {
                DsxColumn _column = value as DsxColumn;

                if (_column.FilterType == EEditType.CheckBox)
                {
                    return _checkBox ? Visibility.Visible : Visibility.Collapsed;
                }
                else
                {
                    return _checkBox ? Visibility.Collapsed : Visibility.Visible;
                }
            }
            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
