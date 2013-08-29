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
    public class VisualGrid : ItemsControl
    {
        #region Col/Row Count

        public int ColumnCount
        {
            get { return (int)GetValue(ColumnCountProperty); }
            set { SetValue(ColumnCountProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ColumnCount.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ColumnCountProperty =
            DependencyProperty.Register("ColumnCount", typeof(int), typeof(VisualGrid), new PropertyMetadata(0, ColumnCount_Changed));


        private static void ColumnCount_Changed(DependencyObject sender, DependencyPropertyChangedEventArgs e) { }

        public int RowCount
        {
            get { return (int)GetValue(RowCountProperty); }
            set { SetValue(RowCountProperty, value); }
        }

        // Using a DependencyProperty as the backing store for RowCount.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RowCountProperty =
            DependencyProperty.Register("RowCount", typeof(int), typeof(VisualGrid), new PropertyMetadata(0, RowCount_Changed));

        private static void RowCount_Changed(DependencyObject sender, DependencyPropertyChangedEventArgs e) { }

        #endregion

        public Boolean ShowGridLines
        {
            get { return (Boolean)GetValue(ShowGridLinesProperty); }
            set { SetValue(ShowGridLinesProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ShowGridLines.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ShowGridLinesProperty =
            DependencyProperty.Register("ShowGridLines", typeof(Boolean), typeof(VisualGrid), new PropertyMetadata(false));



        public Boolean IsEditable
        {
            get { return (Boolean)GetValue(IsEditableProperty); }
            set { SetValue(IsEditableProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsEditable.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsEditableProperty =
            DependencyProperty.Register("IsEditable", typeof(Boolean), typeof(VisualGrid), new PropertyMetadata(false, IsEditableChanged));

        private static void IsEditableChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            if ((Boolean)e.NewValue)
                obj.SetValue(ShowGridLinesProperty, true);
            else obj.SetValue(ShowGridLinesProperty, false);
        }
        #region ItemContainerStyle Property

        public String ColumnFieldName
        {
            get { return (String)GetValue(ColumnFieldNameProperty); }
            set { SetValue(ColumnFieldNameProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ColumnFieldName.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ColumnFieldNameProperty =
            DependencyProperty.Register("ColumnFieldName",
            typeof(String), typeof(VisualGrid),
            new PropertyMetadata("Column", OnColumnFieldNameChanged));

        protected static void OnColumnFieldNameChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as VisualGrid).InitItemContainerStyle();
        }

        public String ColumnSpanFieldName
        {
            get { return (String)GetValue(ColumnSpanFieldNameProperty); }
            set { SetValue(ColumnSpanFieldNameProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ColumnSpanFieldName.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ColumnSpanFieldNameProperty =
            DependencyProperty.Register("ColumnSpanFieldName",
            typeof(String), typeof(VisualGrid),
            new PropertyMetadata("ColumnSpan", OnColumnSpanFieldNameChanged));

        protected static void OnColumnSpanFieldNameChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as VisualGrid).InitItemContainerStyle();
        }

        public String RowFieldName
        {
            get { return (String)GetValue(RowFieldNameProperty); }
            set { SetValue(RowFieldNameProperty, value); }
        }

        // Using a DependencyProperty as the backing store for RowFieldName.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RowFieldNameProperty =
            DependencyProperty.Register("RowFieldName",
            typeof(String), typeof(VisualGrid),
            new PropertyMetadata("Row", OnRowFieldNameChanged));

        protected static void OnRowFieldNameChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as VisualGrid).InitItemContainerStyle();
        }

        public String RowSpanFieldName
        {
            get { return (String)GetValue(RowSpanFieldNameProperty); }
            set { SetValue(RowSpanFieldNameProperty, value); }
        }

        // Using a DependencyProperty as the backing store for RowSpanFieldName.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RowSpanFieldNameProperty =
            DependencyProperty.Register("RowSpanFieldName",
            typeof(String), typeof(VisualGrid),
            new PropertyMetadata("RowSpan", OnRowSpanFieldNameChanged));

        protected static void OnRowSpanFieldNameChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as VisualGrid).InitItemContainerStyle();
        }

        public void InitItemContainerStyle()
        {
            this.ItemContainerStyle = GenerateItemContainerStyle();
        }

        protected virtual Style GenerateItemContainerStyle()
        {
            Style style = new Style();
            style.Setters.Add(new Setter()
            {
                Property = Grid.ColumnProperty,
                Value = new Binding(ColumnFieldName)
            });
            style.Setters.Add(new Setter()
            {
                Property = Grid.ColumnSpanProperty,
                Value = new Binding(ColumnSpanFieldName)
            });
            style.Setters.Add(new Setter()
            {
                Property = Grid.RowProperty,
                Value = new Binding(RowFieldName)
            });
            style.Setters.Add(new Setter()
            {
                Property = Grid.RowSpanProperty,
                Value = new Binding(RowSpanFieldName)
            });
            return style;
        }

        #endregion

        public VisualGrid()
            : base()
        {
            this.Resources.MergedDictionaries.Add(
                new ResourceDictionary()
                {
                    Source = new Uri("pack://application:,,,/Yuhan.WPF.VisualContainer;component/Resources/VisualGrid.xaml")
                });
        }
    }
}
