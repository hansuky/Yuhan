using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Globalization;
using System.Windows;
using System.ComponentModel;
using System.Windows.Controls;

namespace Yuhan.WPF.DsxGridCtrl
{
    #region class - DsxScrollbarVisibleConverter

    [ValueConversion(typeof(DsxColumn), typeof(Visibility))]
    public class DsxScrollbarVisibleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double  _result = 0.0;

            if (value != null && parameter!=null)
            {
                ScrollBarVisibility _value = (ScrollBarVisibility)value;
                string              _param = parameter as String;

                if (_value == ScrollBarVisibility.Auto || _value == ScrollBarVisibility.Visible)
                {
                    if (string.IsNullOrEmpty(_param))
                    {
                        return (double)20.0;
                    }
                    else
                    {
                        _result = System.Convert.ToDouble(_param);
                    }
                }
            }
            return _result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }

    #endregion
}
