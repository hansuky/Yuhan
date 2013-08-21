using System;
using System.Windows.Controls;
using System.Windows;


namespace Yuhan.WPF
{
    public class TreeListView : TreeView
    {


        public GridViewColumnCollection ColumnCollection
        {
            get { return (GridViewColumnCollection)GetValue(ColumnCollectionProperty); }
            set { SetValue(ColumnCollectionProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ColumnCollection.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ColumnCollectionProperty =
            DependencyProperty.Register("ColumnCollection", typeof(GridViewColumnCollection), typeof(TreeListView), new PropertyMetadata(new GridViewColumnCollection()));


        protected override DependencyObject 
                           GetContainerForItemOverride()
        {
            return new TreeListViewItem();
        }

        protected override bool 
                           IsItemItsOwnContainerOverride(object item)
        {
            return item is TreeListViewItem;
        }
        public TreeListView()
            : base()
        {
            this.Resources.MergedDictionaries.Add(
                new ResourceDictionary()
                {
                    Source = new Uri("pack://application:,,,/Yuhan.WPF.TreeListView;component/Resources/TreeListView.xaml")
                });
        }
    }

    public class TreeListViewItem : TreeViewItem
    {
        /// <summary>
        /// Item's hierarchy in the tree
        /// </summary>
        public int Level
        {
            get
            {
                if (_level == -1)
                {
                    TreeListViewItem parent = 
                        ItemsControl.ItemsControlFromItemContainer(this) 
                            as TreeListViewItem;
                    _level = (parent != null) ? parent.Level + 1 : 0;
                }
                return _level;
            }
        }


        protected override DependencyObject 
                           GetContainerForItemOverride()
        {
            return new TreeListViewItem();
        }

        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return item is TreeListViewItem;
        }

        private int _level = -1;
    }

}
