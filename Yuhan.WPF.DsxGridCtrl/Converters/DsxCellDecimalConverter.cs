using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Globalization;
using System.Windows.Controls;
using System.Windows;

namespace Yuhan.WPF.DsxGridCtrl
{
    public class DsxCellDecimalConverter : IValueConverter 
    { 
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) 
        { 
            if (value != null) 
            { 
                return ((decimal)value).ToString("n", CultureInfo.CurrentCulture); 
            } 
            else 
            { 
                return null; 
            } 
        } 
     
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) 
        { 
            throw new NotImplementedException();
        } 
    }
}
