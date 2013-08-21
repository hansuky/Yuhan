using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Diagnostics;
using Yuhan.WPF.DragDrop.DragDropFramework;



namespace Yuhan.WPF.DragDrop.DragDropFrameworkData
{

    /// <summary>
    /// This data consumer creates an item for every entry in a string array.
    /// It is meant to consume a FileDrop or a FileNameW.
    /// 
    /// It knows how to create items for a TabControl, ListBox or TreeView.
    /// </summary>
    public class FileDropConsumer : DataConsumerBase, IDataConsumer
    {

        public FileDropConsumer(string[] dataFormats)
            : base(dataFormats)
        {
        }

        public override DataConsumerActions DataConsumerActions {
            get {
                return
                    //DragDropDataConsumerActions.DragEnter |
                    DataConsumerActions.DragOver |
                    DataConsumerActions.Drop |
                    //DragDropDataConsumerActions.DragLeave |

                    DataConsumerActions.None;
            }
        }

        public override void DropTarget_DragOver(object sender, DragEventArgs e) {
            this.DragOverOrDrop(false, sender, e);
        }

        public override void DropTarget_Drop(object sender, DragEventArgs e) {
            this.DragOverOrDrop(true, sender, e);
        }

        /// <summary>
        /// First determine whether the drag data is supported.
        /// Second determine what type the container is.
        /// Third determine what operation to do (only copy is supported).
        /// And finally handle the actual drop when <code>bDrop</code> is true.
        /// </summary>
        /// <param name="bDrop">True to perform an actual drop, otherwise just return e.Effects</param>
        /// <param name="sender">DragDrop event <code>sender</code></param>
        /// <param name="e">DragDrop event arguments</param>
        private void DragOverOrDrop(bool bDrop, object sender, DragEventArgs e) {
            string[] files = this.GetData(e) as string[];
            if(files != null) {
                e.Effects = DragDropEffects.None;
                ItemsControl dstItemsControl = sender as ItemsControl;  // 'sender' is used when dropped in an empty area
                if(dstItemsControl != null) {
                    foreach(string file in files) {
                        if(sender is TabControl) {
                            if(bDrop) {
                                TabItem item = new TabItem();
                                item.Header = System.IO.Path.GetFileName(file);
                                item.ToolTip = file;
                                dstItemsControl.Items.Insert(0, item);
                                item.IsSelected = true;
                            }
                            e.Effects = DragDropEffects.Copy;
                        }
                        else if(sender is ListBox) {
                            if(bDrop) {
                                ListBoxItem dstItem = Utilities.FindParentControlIncludingMe<ListBoxItem>(e.Source as DependencyObject);
                                ListBoxItem item = new ListBoxItem();
                                item.Content = System.IO.Path.GetFileName(file);
                                item.ToolTip = file;
                                if(dstItem == null)
                                    dstItemsControl.Items.Add(item);    // ... if dropped on an empty area
                                else
                                    dstItemsControl.Items.Insert(dstItemsControl.Items.IndexOf(dstItem), item);

                                item.IsSelected = true;
                                item.BringIntoView();
                            }
                            e.Effects = DragDropEffects.Copy;
                        }
                        else if(sender is TreeView) {
                            if(bDrop) {
                                if(e.Source is ItemsControl)
                                    dstItemsControl = e.Source as ItemsControl; // Dropped on a TreeViewItem
                                TreeViewItem item = new TreeViewItem();
                                item.Header = System.IO.Path.GetFileName(file);
                                item.ToolTip = file;
                                dstItemsControl.Items.Add(item);
                                item.IsSelected = true;
                                item.BringIntoView();
                            }
                            e.Effects = DragDropEffects.Copy;
                        }
                        else {
                            throw new NotSupportedException("The item type is not implemented");
                        }
                        // No need to loop through multiple
                        // files if we're not dropping them
                        if(!bDrop)
                            break;
                    }
                }
                e.Handled = true;
            }
        }
    }
}
