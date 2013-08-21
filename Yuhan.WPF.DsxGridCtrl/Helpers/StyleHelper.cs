using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;

namespace Yuhan.WPF.DsxGridCtrl
{
    public static class StyleHelper
    {
        #region Method - FindStyleValue

        public static object FindStyleValue(object value, string propertyName, DependencyProperty defaultStyleDP, DependencyProperty columnStyleDP)
        {
            object                  _result         = null;

            Style                   _defaultStyle   = null;
            Style                   _columnStyle    = null;

            GridViewColumnHeader    _header         = null;
            DsxColumn               _column         = null;

            if (value is DsxDataGrid)
            {
                _defaultStyle = (Style)((DsxDataGrid)value).GetValue(defaultStyleDP);
            }
            else if (value is GridViewColumnHeader)
            {
                _header     = (GridViewColumnHeader)value;
                _column     = (DsxColumn)_header.Column;
            }
            else
            {
                _column     = (DsxColumn)value;
            }

            if (_column != null)
            {
                _defaultStyle = (Style)_column.DataGrid.GetValue(defaultStyleDP);
                _columnStyle  = (Style)_column.GetValue(columnStyleDP);
            }
            else if(_defaultStyle == null)
            {
                //  remind: padding empty columns have no column assigned in the GridViewColumnHeader

                DsxDataGrid _dataGrid = ElementHelper.FindLogicalParent<DsxDataGrid>(_header);
                if (_dataGrid != null)
                {
                    _defaultStyle = (Style)_dataGrid.GetValue(defaultStyleDP);
                }
            }

            //  apply current (column) Style
            //  fall back to default Style
            _result = _columnStyle.GetStylePropertyValue<object>(propertyName, _defaultStyle, null);

            return _result;
        }
        #endregion
    }
}
