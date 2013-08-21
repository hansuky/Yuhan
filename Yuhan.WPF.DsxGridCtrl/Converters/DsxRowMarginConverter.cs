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
    public class DsxRowMarginConverter : IValueConverter 
    { 
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) 
        { 
            Thickness _result = new Thickness(0);

            if (value == null || parameter == null)
            {
                return _result;
            }

            bool _isBorder = (parameter.ToString()=="Border");
            bool _isMargin = (parameter.ToString()=="Margin");

            if (_isBorder)   _result = new Thickness(0,0,0,0);
            if (_isMargin)   _result = new Thickness(0,0,0,0);

            if (value != null && value is Boolean) 
            { 
                if ((Boolean)value)
                {
                    if (_isBorder)   _result = new Thickness(0,1,0,1);
                    if (_isMargin)   _result = new Thickness(0,0,0,-1);
                }
            } 
            return _result; 
        } 
     
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) 
        { 
            throw new NotImplementedException();
        } 
    }
}
