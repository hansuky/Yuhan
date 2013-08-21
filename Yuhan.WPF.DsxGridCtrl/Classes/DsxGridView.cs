using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Controls.Primitives;

namespace Yuhan.WPF.DsxGridCtrl
{
    public class DsxGridView : GridView
    {
        #region ctors

        public DsxGridView()
        {
            //DefaultStyleKeyProperty.OverrideMetadata(typeof(DsxGridView), new FrameworkPropertyMetadata((typeof(DsxGridView))));
        }
        #endregion

        #region DP - ParentDataGrid

        public static readonly DependencyProperty ParentDataGridProperty =
            DependencyProperty.Register("ParentDataGrid", typeof(DsxDataGrid), typeof(DsxGridView), new PropertyMetadata(null) );

        public DsxDataGrid ParentDataGrid
        {
            get          { return (DsxDataGrid)GetValue(ParentDataGridProperty); }
            internal set { SetValue(ParentDataGridProperty, value); }
        }
        #endregion

        #region DP - ColumnFilterContainerStyle

        public static readonly DependencyProperty ColumnFilterContainerStyleProperty =
            DependencyProperty.Register("ColumnFilterContainerStyle", typeof(Style), typeof(DsxGridView), new PropertyMetadata(null) );

        public Style ColumnFilterContainerStyle
        {
            get { return (Style)GetValue(ColumnFilterContainerStyleProperty); }
            set { SetValue(ColumnFilterContainerStyleProperty, value); }
        }
        #endregion

        #region DP - ColumnFooterContainerStyle

        public static readonly DependencyProperty ColumnFooterContainerStyleProperty =
            DependencyProperty.Register("ColumnFooterContainerStyle", typeof(Style), typeof(DsxGridView), new PropertyMetadata(null) );

        public Style ColumnFooterContainerStyle
        {
            get { return (Style)GetValue(ColumnFooterContainerStyleProperty); }
            set { SetValue(ColumnFooterContainerStyleProperty, value); }
        }
        #endregion
	}
}
