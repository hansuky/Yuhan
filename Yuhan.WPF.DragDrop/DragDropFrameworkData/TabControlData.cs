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
    /// This Data Provider represents TabItems.
    /// Note that custom cursors are used.
    /// When a TabItem is dragged within its
    /// original container, the cursor is an arrow,
    /// otherwise its a custom cursor with an
    /// arrow and a page.
    /// </summary>
    /// <typeparam name="TContainer">Drag source container type</typeparam>
    /// <typeparam name="TObject">Drag source object type</typeparam>
    public class TabControlDataProvider<TContainer, TObject> : DataProviderBase<TContainer, TObject>, IDataProvider
        where TContainer : ItemsControl
        where TObject : TabItem
    {
        static Cursor MovePageCursor = new Cursor(System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream("WpfDragAndDropSmorgasbord.Images.MovePage.cur"));
        static Cursor MovePageNotCursor = new Cursor(System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream("WpfDragAndDropSmorgasbord.Images.MovePageNot.cur"));

        public TabControlDataProvider(string dataFormatString)
            : base(dataFormatString)
        {
        }

        public override DragDropEffects AllowedEffects {
            get {
                return
                    //DragDropEffects.Copy |
                    //DragDropEffects.Scroll |
                    DragDropEffects.Move |	// Move tab from one TabControl to another
                    DragDropEffects.Link |	// Move tabs within the same TabControl

                    DragDropEffects.None;
            }
        }

        public override DataProviderActions DataProviderActions {
            get {
                return
                    //DragDropDataProviderActions.QueryContinueDrag |
                    DataProviderActions.GiveFeedback |
                    //DragDropDataProviderActions.DoDragDrop_Done |

                    DataProviderActions.None;
            }
        }

        public override void DragSource_GiveFeedback(object sender, GiveFeedbackEventArgs e) {
            if(e.Effects == DragDropEffects.Move) { // Move the tab to be the first in the list
                e.UseDefaultCursors = false;
                Mouse.OverrideCursor = MovePageCursor;
                e.Handled = true;
            }
            else if(e.Effects == DragDropEffects.Link) {    // Drag tabs around
                e.UseDefaultCursors = false;
                Mouse.OverrideCursor = Cursors.Arrow;
                e.Handled = true;
            }
            else {
                e.UseDefaultCursors = false;
                Mouse.OverrideCursor = MovePageNotCursor;
                e.Handled = true;
            }
        }

        public override void Unparent() {
            TObject item = this.SourceObject as TObject;
            TContainer container = this.SourceContainer as TContainer;

            Debug.Assert(item != null, "Unparent expects a non-null item");
            Debug.Assert(container != null, "Unparent expects a non-null container");

            if((container != null) && (item != null))
                container.Items.Remove(item);
        }
    }



    /// <summary>
    /// This data consumer looks for TabItems.
    /// When the TabItem is dragged within its original
    /// control, the TabItems are rearranged accordingly.
    /// When dropped, it is inserted as the first TabItem.
    /// </summary>
    /// <typeparam name="TContainer">Drag source and drop destination container type</typeparam>
    /// <typeparam name="TObject">Drag source and drop destination object type</typeparam>
    public class TabControlDataConsumer<TContainer, TObject> : DataConsumerBase, IDataConsumer
        where TContainer : ItemsControl
        where TObject : TabItem
    {

        public TabControlDataConsumer(string[] dataFormats)
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
        /// Next check if it's a move within the same TabControl,
        /// and rearrange the TabItems.
        /// Finally handle the actual drop when <code>bDrop</code> is true.
        /// </summary>
        /// <param name="bDrop">True to perform an actual drop, otherwise just return e.Effects</param>
        /// <param name="sender">DragDrop event <code>sender</code></param>
        /// <param name="e">DragDrop event arguments</param>
        private void DragOverOrDrop(bool bDrop, object sender, DragEventArgs e) {
            TabControlDataProvider<TContainer, TObject> dataProvider = this.GetData(e) as TabControlDataProvider<TContainer, TObject>;
            if(dataProvider != null) {
                TContainer dragSourceContainer = dataProvider.SourceContainer as TContainer;
                TObject dragSourceObject = dataProvider.SourceObject as TObject;
                Debug.Assert(dragSourceObject != null);
                Debug.Assert(dragSourceContainer != null);

                TContainer dropContainer = Utilities.FindParentControlIncludingMe<TContainer>(e.Source as DependencyObject);
                TObject dropTarget = e.Source as TObject;

                if((dragSourceContainer == dropContainer) && (dropTarget != null)) {    // Reorder within same container
                    int srcIndex = dragSourceContainer.Items.IndexOf(dragSourceObject);
                    int dstIndex = dropContainer.Items.IndexOf(dropTarget);
                    if(srcIndex != dstIndex) {
                        // Only move when there's no chance of oscillation
                        bool doMove = true;
                        if(dragSourceObject.ActualWidth < (dropTarget.ActualWidth)) {
                            Point point = e.GetPosition(dropTarget);
                            if(srcIndex < dstIndex) {
                                doMove = point.X > ((dropTarget.ActualWidth - dragSourceObject.ActualWidth));
                            }
                            else {
                                doMove = point.X < dragSourceObject.ActualWidth;
                            }
                        }
                        if(doMove) {
                            dataProvider.Unparent();
                            dropContainer.Items.Insert(dstIndex, dragSourceObject);
                            dragSourceObject.IsSelected = true;
                            //Debug.WriteLine("DragOverOrDrop doMove=True srcIndex=" + srcIndex.ToString() + " dstIndex=" + dstIndex.ToString());
                        }
                    }
                    e.Effects = DragDropEffects.Link;
                    e.Handled = true;
#if PRINT2OUTPUT
                    Debug.WriteLine("   Link0");
#endif
                }
                else if(dropContainer != null) {    // Move to destination container as 1st TabItem
                    if(bDrop) {
                        //srcTabControl.Items.Remove(srcTabItem);
                        dataProvider.Unparent();
                        dropContainer.Items.Insert(0, dragSourceObject);
                        dragSourceObject.IsSelected = true;
                    }
                    e.Effects = DragDropEffects.Move;
                    e.Handled = true;
#if PRINT2OUTPUT
                    Debug.WriteLine("  Move0");
#endif
                }
                else {
                    e.Effects = DragDropEffects.None;
                    e.Handled = true;
#if PRINT2OUTPUT
                    Debug.WriteLine("  None0");
#endif
                }
            }
        }
    }
}
