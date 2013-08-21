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
    /// This data consumer looks for a ListBoxItem
    /// in a ListBox container.
    /// The ListBoxItem is added as the target's child,
    /// or it is inserted before the target if the Shift
    /// key is pressed.  If the ListBoxItem is dropped
    /// in empty space, it is added to the end of the
    /// TreeView's items.
    /// </summary>
    /// <typeparam name="TSourceContainer">Drag data source container type</typeparam>
    /// <typeparam name="TSourceObject">Drag data source object type</typeparam>
    public class ListBoxItemToTreeViewItem<TSourceContainer, TSourceObject> : DataConsumerBase, IDataConsumer
        where TSourceContainer : ListBox
        where TSourceObject : ListBoxItem
    {

        public ListBoxItemToTreeViewItem(string[] dataFormats)
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
        /// Add the item as the drop target's child when Shift is not pressed,
        /// or insert the item before the drop target when Shift is pressed.
        /// When there is no drop target (dropped on empty space),
        /// add to the end of the items.
        /// </summary>
        /// <param name="bDrop">True to perform an actual drop, otherwise just return e.Effects</param>
        /// <param name="sender">DragDrop event <code>sender</code></param>
        /// <param name="e">DragDrop event arguments</param>
        private void DragOverOrDrop(bool bDrop, object sender, DragEventArgs e) {
            ListBoxDataProvider<TSourceContainer, TSourceObject> dataProvider = this.GetData(e) as ListBoxDataProvider<TSourceContainer, TSourceObject>;
            if(dataProvider != null) {
                TSourceContainer dragSourceContainer = dataProvider.SourceContainer as TSourceContainer;
                TSourceObject dragSourceObject = dataProvider.SourceObject as TSourceObject;
                Debug.Assert(dragSourceContainer != null);
                Debug.Assert(dragSourceObject != null);

                ItemsControl dropContainer = Utilities.FindParentControlIncludingMe<ItemsControl>(sender as DependencyObject);
                Debug.Assert(dropContainer != null);
                TreeViewItem dropTarget = e.Source as TreeViewItem;

                TreeViewItem newTvi = null;
                if(bDrop) {
                    dataProvider.Unparent();
                    newTvi = new TreeViewItem();
                    newTvi.Header = dragSourceObject.Content;
                }

                if(dropTarget == null) {
                    if(bDrop) {
                        dropContainer.Items.Add(newTvi);
                    }
                    e.Effects = DragDropEffects.Move;
                    e.Handled = true;
                }
                else {
                    if((dataProvider.KeyStates & DragDropKeyStates.ShiftKey) != 0) {    // As sibling
                        if(bDrop) {
                            ItemsControl shiftDropTarget = Utilities.FindParentControlExcludingMe<ItemsControl>(dropTarget);
                            Debug.Assert(shiftDropTarget != null);
                            shiftDropTarget.Items.Insert(shiftDropTarget.Items.IndexOf(dropTarget), newTvi);
                        }
                        e.Effects = DragDropEffects.Link;
                        e.Handled = true;
                    }
                    else {  // As child
                        if(bDrop) {
                            dropTarget.Items.Add(newTvi);
                        }
                        e.Effects = DragDropEffects.Move;
                        e.Handled = true;
                    }
                }

                if(bDrop) {
                    newTvi.IsSelected = true;
                    newTvi.BringIntoView();
                }
            }
        }
    }
}
