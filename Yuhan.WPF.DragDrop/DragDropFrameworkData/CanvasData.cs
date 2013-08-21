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
using System.Windows.Shapes;
using Yuhan.WPF.DragDrop.DragDropFramework;



namespace Yuhan.WPF.DragDrop.DragDropFrameworkData
{

    /// <summary>
    /// This Data Provider represents items found on a Canvas,
    /// allowing them to be drag data.
    /// Note that text specific to the object is also added to the drag data;
    /// this allows canvas items to be dragged onto the Rich Text Box.
    /// </summary>
    /// <typeparam name="TContainer">Drag source container type</typeparam>
    /// <typeparam name="TObject">Drag source object type</typeparam>
    public class CanvasDataProvider<TContainer, TObject> : DataProviderBase<TContainer, TObject>, IDataProvider
        where TContainer : Canvas
        where TObject : UIElement
    {

        public CanvasDataProvider(string dataFormatString) :
            base(dataFormatString)
        {
        }

        /// <summary>
        /// Return true so an addorner is added when an item is dragged
        /// </summary>
        public override bool AddAdorner { get { return true; } }

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

        public override bool IsSupportedContainerAndObject(bool initFlag, object dragSourceContainer, object dragSourceObject, object dragOriginalSourceObject) {
            TObject sourceObject = dragSourceObject as TObject;
            // When an image button is clicked,
            // most of the time the image is the <code>e.Source</code>.
            // So when _SourceObject is null, search for a TObject parent.
            if(sourceObject == null) {
                // Image buttons can return the image as the source, so look for the button
                sourceObject = Utilities.FindParentControlExcludingMe<TObject>(dragSourceObject as DependencyObject);
            }

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

        /// <summary>
        /// Not only add the DataProvider class, also add a string
        /// </summary>
        public override void SetData(ref DataObject data) {
            // Set default data
            System.Diagnostics.Debug.Assert(data.GetDataPresent(this.SourceDataFormat) == false, "Shouldn't set data more than once");
            data.SetData(this.SourceDataFormat, this);

            // Look for a System.String
            string textString = null;

            if(this.SourceObject is Rectangle) {
                Rectangle rect = (Rectangle)this.SourceObject;
                if(rect.Fill != null)
                    textString = rect.Fill.ToString();
            }
            else if(this.SourceObject is TextBlock) {
                TextBlock textBlock = (TextBlock)this.SourceObject;
                textString = textBlock.Text;
            }
            else if(this.SourceObject is Button) {
                Button button = (Button)this.SourceObject;
                if(button.ToolTip != null)
                    textString = button.ToolTip.ToString();
            }

            if(textString != null)
                data.SetData(textString);
        }

        public override void DragSource_GiveFeedback(object sender, GiveFeedbackEventArgs e) {
            if(e.Effects == DragDropEffects.Move) {
                e.UseDefaultCursors = true;
                e.Handled = true;
            }
        }

        public override void Unparent() {
            TObject item = this.SourceObject as TObject;
            TContainer panel = this.SourceContainer as TContainer;

            Debug.Assert(item != null, "Unparent expects a non-null item");
            Debug.Assert(panel != null, "Unparent expects a non-null panel");

            if((panel != null) && (item != null))
                panel.Children.Remove(item);
        }
    }



    /// <summary>
    /// This data consumer looks for drag data coming from
    /// a canvas (of type TContainer) and
    /// a drag source data object of type TObject.
    /// When dropped, it moves the data object to the
    /// mouse drop location.
    /// </summary>
    /// <typeparam name="TContainer">Drag source and drop destination container type</typeparam>
    /// <typeparam name="TObject">Drag source and drop destination object type</typeparam>
    public class CanvasDataConsumer<TContainer, TObject> : DataConsumerBase, IDataConsumer
        where TContainer : Canvas
        where TObject : UIElement
    {

        public CanvasDataConsumer(string[] dataFormats)
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
        /// Second determine what operation to do (copy, move, link).
        /// And finally handle the actual drop when <code>bDrop</code> is true.
        /// </summary>
        /// <param name="bDrop">True to perform an actual drop, otherwise just return e.Effects</param>
        /// <param name="sender">DragDrop event <code>sender</code></param>
        /// <param name="e">DragDrop event arguments</param>
        private void DragOverOrDrop(bool bDrop, object sender, DragEventArgs e) {
            CanvasDataProvider<TContainer, TObject> dataProvider = this.GetData(e) as CanvasDataProvider<TContainer, TObject>;
            if(dataProvider != null) {
                TObject dragSourceObject = dataProvider.SourceObject as TObject;
                Debug.Assert(dragSourceObject != null);

                TContainer dropContainer = sender as TContainer;

                if(dropContainer != null) {
                    if(bDrop) {
                        dataProvider.Unparent();
                        dropContainer.Children.Add(dragSourceObject);

                        Point dropPosition = e.GetPosition(dropContainer);
                        Point objectOrigin = dataProvider.StartPosition;
                        Canvas.SetLeft(dragSourceObject, dropPosition.X - objectOrigin.X);
                        Canvas.SetTop(dragSourceObject, dropPosition.Y - objectOrigin.Y);
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
