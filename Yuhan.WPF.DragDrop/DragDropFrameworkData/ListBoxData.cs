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
    /// This Data Provider represents ListBoxItems.
    /// </summary>
    /// <typeparam name="TContainer">Drag source container type</typeparam>
    /// <typeparam name="TObject">Drag source object type</typeparam>
    public class ListBoxDataProvider<TContainer, TObject> : DataProviderBase<TContainer, TObject>, IDataProvider
        where TContainer : ItemsControl
        where TObject : FrameworkElement
    {

        public ListBoxDataProvider(string dataFormatString) :
            base(dataFormatString)
        {
        }

        public override DragDropEffects AllowedEffects {
            get {
                return
                    //DragDropEffects.Copy |
                    //DragDropEffects.Scroll |
                    DragDropEffects.Move |	// Move ListBoxItem
                    DragDropEffects.Link |  // Needed for moving ListBoxItem as a TreeView sibling

                    DragDropEffects.None;
            }
        }

        public override DataProviderActions DataProviderActions {
            get {
                return
                    DataProviderActions.QueryContinueDrag | // Need Shift key info (for TreeView)
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
            else if(e.Effects == DragDropEffects.Link) {    // ... when Shift key is pressed
                e.UseDefaultCursors = true;
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
    /// This data consumer looks for ListBoxItems.
    /// The ListBoxItem is inserted before the
    /// target ListBoxItem or at the end of the
    /// list if dropped on empty space.
    /// </summary>
    /// <typeparam name="TContainer">Drag source and drop destination container type</typeparam>
    /// <typeparam name="TObject">Drag source and drop destination object type</typeparam>
    public class ListBoxDataConsumer<TContainer, TObject> : DataConsumerBase, IDataConsumer
        where TContainer : ItemsControl
        where TObject : ListBoxItem
    {

        public ListBoxDataConsumer(string[] dataFormats)
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
        /// </summary>
        /// <param name="bDrop">True to perform an actual drop, otherwise just return e.Effects</param>
        /// <param name="sender">DragDrop event <code>sender</code></param>
        /// <param name="e">DragDrop event arguments</param>
        private void DragOverOrDrop(bool bDrop, object sender, DragEventArgs e) {
            ListBoxDataProvider<TContainer, TObject> dataProvider = this.GetData(e) as ListBoxDataProvider<TContainer, TObject>;
            if(dataProvider != null) {
                TContainer dragSourceContainer = dataProvider.SourceContainer as TContainer;
                TObject dragSourceObject = dataProvider.SourceObject as TObject;
                Debug.Assert(dragSourceObject != null);
                Debug.Assert(dragSourceContainer != null);

                TContainer dropContainer = Utilities.FindParentControlIncludingMe<TContainer>(e.Source as DependencyObject);
                TObject dropTarget = e.Source as TObject;

                if(dropContainer != null) {
                    if(bDrop) {
                        dataProvider.Unparent();
                        if(dropTarget == null)
                            dropContainer.Items.Add(dragSourceObject);
                        else
                            dropContainer.Items.Insert(dropContainer.Items.IndexOf(dropTarget), dragSourceObject);

                        dragSourceObject.IsSelected = true;
                        dragSourceObject.BringIntoView();
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
