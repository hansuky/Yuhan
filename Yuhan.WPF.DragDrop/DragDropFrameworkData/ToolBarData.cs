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
    /// This Data Provider represents items found on a ToolBar.
    /// Note that text specific to the object is also added to the drag data;
    /// this allows canvas items to be dragged onto the Rich Text Box.
    /// </summary>
    /// <typeparam name="TContainer">Drag source container type</typeparam>
    /// <typeparam name="TObject">Drag source object type</typeparam>
    public class ToolBarDataProvider<TContainer, TObject> : DataProviderBase<TContainer, TObject>, IDataProvider
        where TContainer : ItemsControl
        where TObject : UIElement
    {

        public ToolBarDataProvider(string dataFormatString) :
            base(dataFormatString)
        {
        }

        public override bool IsSupportedContainerAndObject(bool initFlag, object dragSourceContainer, object dragSourceObject, object dragOriginalSourceObject) {
            TObject sourceObject = dragSourceObject as TObject;
            // When an image button is clicked,
            // most of the time the image is the <code>e.Source</code>.
            // So when _SourceObject is null, search for a TObject parent.
            if(sourceObject == null)
                sourceObject = Utilities.FindParentControlExcludingMe<TObject>(dragSourceObject as DependencyObject);

            if(initFlag) {
                // Init DataProvider variables
                this.Init();
                this.SourceContainer = dragSourceContainer;
                this.SourceObject = sourceObject;
                this.OriginalSourceObject = dragOriginalSourceObject;
            }

            return
                (dragSourceContainer is TContainer) &&
                (sourceObject != null)
                ;
        }

        public override DragDropEffects AllowedEffects {
            get {
                return
                    //DragDropEffects.Copy |
                    //DragDropEffects.Scroll |
                    DragDropEffects.Move |
                    DragDropEffects.Link |  // Indicates a ToolBar "insert"

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
            TContainer container = (TContainer)this.SourceContainer;

            Debug.Assert(item != null, "Unparent expects a non-null item");
            Debug.Assert(container != null, "Unparent expects a non-null container");

            if((container != null) && (item != null))
                container.Items.Remove(item);
        }
    }



    /// <summary>
    /// This data consumer looks for Buttons coming from a ToolBar.
    /// When dropped, it either inserts the button (if drop target
    /// is a button) or moves the button to the end of the ToolBar.
    /// </summary>
    /// <typeparam name="TContainer">Drag source and drop destination container type</typeparam>
    /// <typeparam name="TObject">Drag source and drop destination object type</typeparam>
    public class ToolBarDataConsumer<TContainer, TObject> : DataConsumerBase, IDataConsumer
        where TContainer : ItemsControl
        where TObject : UIElement
    {

        public ToolBarDataConsumer(string[] dataFormats)
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
        /// Second determine what operation to do (move, link).
        /// And finally handle the actual drop when <code>bDrop</code> is true.
        /// Insert the button if the target is another button, otherwise
        /// just add it to the end of the list.
        /// </summary>
        /// <param name="bDrop">True to perform an actual drop, otherwise just return e.Effects</param>
        /// <param name="sender">DragDrop event <code>sender</code></param>
        /// <param name="e">DragDrop event arguments</param>
        private void DragOverOrDrop(bool bDrop, object sender, DragEventArgs e) {
            ToolBarDataProvider<TContainer, TObject> dataProvider = this.GetData(e) as ToolBarDataProvider<TContainer, TObject>;
            if(dataProvider != null) {
                TContainer dragSourceContainer = dataProvider.SourceContainer as TContainer;
                TObject dragSourceObject = dataProvider.SourceObject as TObject;
                Debug.Assert(dragSourceObject != null);
                Debug.Assert(dragSourceContainer != null);

                TContainer dropContainer = sender as TContainer;
                TObject dropTarget = e.Source as TObject;
                if(dropTarget == null)
                    dropTarget = Utilities.FindParentControlExcludingMe<TObject>(e.Source as DependencyObject);

                if(dropContainer != null) {
                    if(bDrop) {
                        dataProvider.Unparent();
                        if(dropTarget == null)
                            dropContainer.Items.Add(dragSourceObject);
                        else
                            dropContainer.Items.Insert(dropContainer.Items.IndexOf(dropTarget), dragSourceObject);
                    }
                    e.Effects = (dropTarget == null) ? DragDropEffects.Move : DragDropEffects.Link;
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
