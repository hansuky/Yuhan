using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace Yuhan.WPF.VisualContainer
{
    public class VisualContainer : ItemsControl
    {
        #region Col/Row Count

        public int ColumnCount
        {
            get { return (int)GetValue(ColumnCountProperty); }
            set { SetValue(ColumnCountProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ColumnCount.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ColumnCountProperty =
            DependencyProperty.Register("ColumnCount", typeof(int), typeof(VisualContainer), new PropertyMetadata(0, ColumnCount_Changed));


        private static void ColumnCount_Changed(DependencyObject sender, DependencyPropertyChangedEventArgs e) { }

        public int RowCount
        {
            get { return (int)GetValue(RowCountProperty); }
            set { SetValue(RowCountProperty, value); }
        }

        // Using a DependencyProperty as the backing store for RowCount.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RowCountProperty =
            DependencyProperty.Register("RowCount", typeof(int), typeof(VisualContainer), new PropertyMetadata(0, RowCount_Changed));

        private static void RowCount_Changed(DependencyObject sender, DependencyPropertyChangedEventArgs e) { }

        #endregion

        public Boolean ShowGridLines
        {
            get { return (Boolean)GetValue(ShowGridLinesProperty); }
            set { SetValue(ShowGridLinesProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ShowGridLines.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ShowGridLinesProperty =
            DependencyProperty.Register("ShowGridLines", typeof(Boolean), typeof(VisualContainer), new PropertyMetadata(false));



        public Boolean IsEditable
        {
            get { return (Boolean)GetValue(IsEditableProperty); }
            set { SetValue(IsEditableProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsEditable.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsEditableProperty =
            DependencyProperty.Register("IsEditable", typeof(Boolean), typeof(VisualContainer), new PropertyMetadata(false, IsEditableChanged));


        private static void IsEditableChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            if ((Boolean)e.NewValue)
                obj.SetValue(ShowGridLinesProperty, true);
            else obj.SetValue(ShowGridLinesProperty, false);
        }

        public VisualContainer()
            : base()
        {
            this.Resources.MergedDictionaries.Add(
                new ResourceDictionary()
                {
                    Source = new Uri("pack://application:,,,/Yuhan.WPF.VisualContainer;component/Resources/VisualContainer.xaml")
                });
        }
    }
}
