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
    /// This data consumer looks for Buttons coming from a ToolBar.
    /// When dropped, the button is moved from the ToolBar
    /// to the Canvas.
    /// </summary>
    /// <typeparam name="TContainer">Drag data source container type</typeparam>
    /// <typeparam name="TObject">Drag data source object type</typeparam>
    public class ToolbarButtonToCanvasButton<TContainer, TObject> : DataConsumerBase, IDataConsumer
        where TContainer : ItemsControl
        where TObject : Button
    {

        public ToolbarButtonToCanvasButton(string[] dataFormats)
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
        /// Second determine whether or not a Move can be done.
        /// And finally handle the actual drop when <code>bDrop</code> is true.
        /// </summary>
        /// <param name="bDrop">True to perform an actual drop, otherwise just return e.Effects</param>
        /// <param name="sender">DragDrop event <code>sender</code></param>
        /// <param name="e">DragDrop event arguments</param>
        private void DragOverOrDrop(bool bDrop, object sender, DragEventArgs e) {
            ToolBarDataProvider<TContainer, TObject> dataProvider = this.GetData(e) as ToolBarDataProvider<TContainer, TObject>;
            if(dataProvider != null) {
                TContainer dragSourceContainer = dataProvider.SourceContainer as TContainer;
                TObject dragSourceObject = dataProvider.SourceObject as TObject;
                if(dragSourceObject == null)
                    dragSourceObject = Utilities.FindParentControlIncludingMe<TObject>(dataProvider.SourceObject as DependencyObject);
                Debug.Assert(dragSourceObject != null);
                Debug.Assert(dragSourceContainer != null);

                Panel dropContainer = sender as Panel;

                if(dropContainer != null) {
                    if(bDrop) {
                        dataProvider.Unparent();
                        Point containerPoint = e.GetPosition(dropContainer);
                        Point objectPoint = dataProvider.StartPosition;
#if REUSE_SAME_BUTTON //|| true
                        dropContainer.Children.Add(dragSourceObject);
                        Canvas.SetLeft(dragSourceObject, containerPoint.X - objectPoint.X);
                        Canvas.SetTop(dragSourceObject, containerPoint.Y - objectPoint.Y);
#else
                        Button oldButton = dragSourceObject as Button;
                        Button newButton = new Button();
                        newButton.Content = Utilities.CloneElement(oldButton.Content);
                        newButton.ToolTip = oldButton.ToolTip;
                        dropContainer.Children.Add(newButton);
                        Canvas.SetLeft(newButton, containerPoint.X - objectPoint.X);
                        Canvas.SetTop(newButton, containerPoint.Y - objectPoint.Y);
#endif
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
