#define PRINT2BUFFER
#define PRINT2OUTPUT

using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using System.Xml;
using System.Diagnostics;
using System.Windows.Input;
using System.Windows.Media;
using Yuhan.WPF.DragDrop.DragDropFramework;



namespace Yuhan.WPF.DragDrop.DragDropFrameworkData
{

    /// <summary>
    /// This data consumer looks for TreeViewItems.
    /// The item is inserted before the
    /// target ListBoxItem or at the end of the
    /// list if dropped on empty space.
    /// 
    /// Note that only TreeViewItems with no children can be moved.
    /// </summary>
    /// <typeparam name="TSourceContainer">Drag data source container type</typeparam>
    /// <typeparam name="TSourceObject">Drag data source object type</typeparam>
    public class TreeViewItemToListBoxItem<TSourceContainer, TSourceObject> : DataConsumerBase, IDataConsumer
        where TSourceContainer : ItemsControl
        where TSourceObject : TreeViewItem
    {

        public TreeViewItemToListBoxItem(string[] dataFormats)
            : base(dataFormats)
        {
        }

        public override DataConsumerActions DataConsumerActions {
            get {
                return
                    DataConsumerActions.DragEnter |
                    DataConsumerActions.DragOver |
                    DataConsumerActions.Drop |
                    //DragDropDataConsumerActions.DragLeave |

                    DataConsumerActions.None;
            }
        }

        public override void DropTarget_DragEnter(object sender, DragEventArgs e) {
            this.DragOverOrDrop(false, sender, e);
        }

        public override void DropTarget_DragOver(object sender, DragEventArgs e) {
            this.DragOverOrDrop(false, sender, e);
        }

        public override void DropTarget_Drop(object sender, DragEventArgs e) {
            this.DragOverOrDrop(true, sender, e);
        }

        /// <summary>
        /// First determine whether the drag data is supported.
        /// Finally handle the actual drop when <code>bDrop</code> is true.
        /// Insert the item before the drop target.  When there is no drop
        /// target (dropped on empty space), add to the end of the items.
        /// 
        /// Note that only TreeViewItems with no children can be moved.
        /// </summary>
        /// <param name="bDrop">True to perform an actual drop, otherwise just return e.Effects</param>
        /// <param name="sender">DragDrop event <code>sender</code></param>
        /// <param name="e">DragDrop event arguments</param>
        private void DragOverOrDrop(bool bDrop, object sender, DragEventArgs e) {
            TreeViewDataProvider<TSourceContainer, TSourceObject> dataProvider = this.GetData(e) as TreeViewDataProvider<TSourceContainer, TSourceObject>;
            if(dataProvider != null) {
                TSourceObject dragSourceObject = dataProvider.SourceObject as TSourceObject;
                TSourceContainer dragSourceContainer = dataProvider.SourceContainer as TSourceContainer;
                Debug.Assert(dragSourceObject != null);
                Debug.Assert(dragSourceContainer != null);

                ListBox dropContainer = Utilities.FindParentControlIncludingMe<ListBox>(e.Source as DependencyObject);
                ListBoxItem dropTarget = Utilities.FindParentControlIncludingMe<ListBoxItem>(e.Source as DependencyObject);

                if(!dragSourceObject.HasItems) {    // TreeViewItem must be a leaf
                    if(bDrop) {
                        dataProvider.Unparent();

                        ListBoxItem item = new ListBoxItem();
                        item.Content = dragSourceObject.Header;
                        item.ToolTip = dragSourceObject.ToolTip;
                        if(dropTarget == null)
                            dropContainer.Items.Add(item);
                        else
                            dropContainer.Items.Insert(dropContainer.Items.IndexOf(dropTarget), item);

                        item.IsSelected = true;
                        item.BringIntoView();
                    }
                    e.Effects = DragDropEffects.Move;
                    e.Handled = true;
                }
                else {
                    e.Effects = DragDropEffects.None;
                    e.Handled = true;
                }
            }
        }
    }
}
