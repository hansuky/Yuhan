using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Globalization;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Media;

namespace Yuhan.WPF.DsxGridCtrl
{
    public class DsxRowColorConverter : IValueConverter 
    { 
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) 
        { 
            Brush _result = Brushes.Transparent;

            if (value == null)
            {
                return _result;
            }
            ListViewItem  _listItem = (ListViewItem)value;
            ListView      _listView = ItemsControl.ItemsControlFromItemContainer(_listItem) as ListView;

            if (_listView.AlternationCount>0)
            {
                int         _index      = (int)_listItem.GetValue(ItemsControl.AlternationIndexProperty);
                DsxDataGrid _dataGrid   = (_listView.View as DsxGridView).ParentDataGrid;

                if (_dataGrid.AlternatingRowBrushes != null && _dataGrid.AlternatingRowBrushes.Count > _index)
                {
                    _result = _dataGrid.AlternatingRowBrushes[_index];
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
