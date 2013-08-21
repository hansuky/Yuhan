using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace Yuhan.WPF
{
    public class TreeGridViewColumn : GridViewColumn
    {
        public Boolean Expandable
        {
            get { return (Boolean)GetValue(ExpandableProperty); }
            set { SetValue(ExpandableProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Expandable.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ExpandableProperty =
            DependencyProperty.Register("Expandable", typeof(Boolean), typeof(TreeGridViewColumn), new PropertyMetadata(false));


        public TreeGridViewColumn()
            : base()
        {
            
        }
    }
}
