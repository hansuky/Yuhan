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
    /// This Data Provider represents TreeViewItems.
    /// 
    /// Note that a TreeViewItem's container can be
    /// either a TreeView or another TreeViewItem.
    /// </summary>
    /// <typeparam name="TContainer">Drag source container type</typeparam>
    /// <typeparam name="TObject">Drag source object type</typeparam>
    public class TreeViewDataProvider<TContainer, TObject> : DataProviderBase<TContainer, TObject>, IDataProvider
        where TContainer : ItemsControl
        where TObject : ItemsControl
    {

        public TreeViewDataProvider(string dataFormatString)
            : base(dataFormatString)
        {
        }

        public override DragDropEffects AllowedEffects {
            get {
                return
                    //DragDropEffects.Copy |
                    //DragDropEffects.Scroll |
                    DragDropEffects.Move |	// Move TreeItem
                    DragDropEffects.Link |  // Move TreeItem as sibling

                    DragDropEffects.None;
            }
        }

        public override DataProviderActions DataProviderActions {
            get {
                return
                    DataProviderActions.QueryContinueDrag | // Need Shift key info
                    DataProviderActions.GiveFeedback |
                    //DragDropDataProviderActions.DoDragDrop_Done |

                    DataProviderActions.None;
            }
        }

        public override void DragSource_GiveFeedback(object sender, GiveFeedbackEventArgs e) {
            if(e.Effects == DragDropEffects.Move) {
                e.UseDefaultCursors = true;
                e.Handled = true;
            }
            else if(e.Effects == DragDropEffects.Link) {
                e.UseDefaultCursors = true;
                e.Handled = true;
            }
        }

        public override void Unparent() {
            TObject item = this.SourceObject as TObject;
            // 'container' can be either TreeView or another TreeViewItem
            TContainer container = Utilities.FindParentControlExcludingMe<TContainer>(item);

            Debug.Assert(item != null, "Unparent expects a non-null item");
            Debug.Assert(container != null, "Unparent expects a non-null container");

            if((container != null) && (item != null))
                container.Items.Remove(item);
        }
    }



    /// <summary>
    /// This data consumer looks for TreeViewItems.
    /// The TreeViewItem is added as either a sibling or
    /// a child, depending on the state of the Shift key.
    /// </summary>
    /// <typeparam name="TContainer">Drag source and drop destination container type</typeparam>
    /// <typeparam name="TObject">Drag source and drop destination object type</typeparam>
    public class TreeViewDataConsumer<TContainer, TObject> : DataConsumerBase, IDataConsumer
        where TContainer : ItemsControl
        where TObject : ItemsControl
    {

        public TreeViewDataConsumer(string[] dataFormats)
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
        /// 
        /// Note that the source object cannot be an ancestor of the drop target.
        /// </summary>
        /// <param name="bDrop">True to perform an actual drop, otherwise just return e.Effects</param>
        /// <param name="sender">DragDrop event <code>sender</code></param>
        /// <param name="e">DragDrop event arguments</param>
        private void DragOverOrDrop(bool bDrop, object sender, DragEventArgs e) {
            TreeViewDataProvider<TContainer, TObject> dataProvider = this.GetData(e) as TreeViewDataProvider<TContainer, TObject>;
            if(dataProvider != null) {
                TContainer dragSourceContainer = dataProvider.SourceContainer as TContainer;
                TreeViewItem dragSourceObject = dataProvider.SourceObject as TreeViewItem;
                Debug.Assert(dragSourceContainer != null);
                Debug.Assert(dragSourceObject != null);

                TContainer dropContainer = Utilities.FindParentControlIncludingMe<TContainer>(sender as DependencyObject);
                Debug.Assert(dropContainer != null);
                TObject dropTarget = e.Source as TObject;

                if(dropTarget == null) {
                    if(bDrop) {
                        dataProvider.Unparent();
                        dropContainer.Items.Add(dragSourceObject);
                    }
                    e.Effects = DragDropEffects.Move;
                    e.Handled = true;
#if PRINT2OUTPUT
                    Debug.WriteLine("  Move0");
#endif
                }
                else {
                    bool IsAncestor = dragSourceObject.IsAncestorOf(dropTarget);
                    if((dataProvider.KeyStates & DragDropKeyStates.ShiftKey) != 0) {
                        ItemsControl shiftDropTarget = Utilities.FindParentControlExcludingMe<ItemsControl>(dropTarget);
                        Debug.Assert(shiftDropTarget != null);
                        if(!IsAncestor) {
                            if(bDrop) {
                                dataProvider.Unparent();
                                Debug.Assert(shiftDropTarget != null);
                                shiftDropTarget.Items.Insert(shiftDropTarget.Items.IndexOf(dropTarget), dragSourceObject);
                            }
                            e.Effects = DragDropEffects.Link;
                            e.Handled = true;
#if PRINT2OUTPUT
                            Debug.WriteLine("  Link1");
#endif
                        }
                        else {
                            e.Effects = DragDropEffects.None;
                            e.Handled = true;
#if PRINT2OUTPUT
                            Debug.WriteLine("  None1");
#endif
                        }
                    }
                    else {
                        if(!IsAncestor && (dragSourceObject != dropTarget)) {
                            if(bDrop) {
                                dataProvider.Unparent();
                                dropTarget.Items.Add(dragSourceObject);
                            }
                            e.Effects = DragDropEffects.Move;
                            e.Handled = true;
#if PRINT2OUTPUT
                            Debug.WriteLine("  Move2");
#endif
                        }
                        else {
                            e.Effects = DragDropEffects.None;
                            e.Handled = true;
#if PRINT2OUTPUT
                            Debug.WriteLine("  None2");
#endif
                        }
                    }
                }
                if(bDrop && e.Handled && (e.Effects != DragDropEffects.None)) {
                    dragSourceObject.IsSelected = true;
                    dragSourceObject.BringIntoView();
                }
            }
        }
    }
}
