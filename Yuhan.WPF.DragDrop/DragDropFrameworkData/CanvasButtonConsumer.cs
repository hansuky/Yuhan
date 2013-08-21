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
    /// This data consumer looks for drag data coming from
    /// a drag source container of type TContainer and
    /// a drag source data object of type TObject.
    /// It creates a new button using the contents of the
    /// old button and adds the new button to the
    /// drop target's container.
    /// </summary>
    /// <typeparam name="TContainer">Drag data source container type</typeparam>
    /// <typeparam name="TObject">Drag data source object type</typeparam>
    public class CanvasButtonToToolbarButton<TContainer, TObject> : DataConsumerBase, IDataConsumer
        where TContainer : Canvas
        where TObject : Button
    {

        public CanvasButtonToToolbarButton(string[] dataFormats)
            : base(dataFormats)
        {
        }

        public override DragDropFramework.DataConsumerActions DataConsumerActions {
            get {
                return
                    DragDropFramework.DataConsumerActions.DragEnter |
                    DragDropFramework.DataConsumerActions.DragOver |
                    DragDropFramework.DataConsumerActions.Drop |
                    //Ddf.DragDropDataConsumerActions.DragLeave |

                    DragDropFramework.DataConsumerActions.None;
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
        /// 
        /// Note that a new button needs to be created for the toolbar.
        /// </summary>
        /// <param name="bDrop">True to perform an actual drop, otherwise just return e.Effects</param>
        /// <param name="sender">DragDrop event <code>sender</code></param>
        /// <param name="e">DragDrop event arguments</param>
        private void DragOverOrDrop(bool bDrop, object sender, DragEventArgs e) {
            CanvasDataProvider<TContainer, TObject> dataProvider = this.GetData(e) as CanvasDataProvider<TContainer, TObject>;
            if(dataProvider != null) {
                TContainer dragSourceContainer = dataProvider.SourceContainer as TContainer;
                TObject dragSourceObject = dataProvider.SourceObject as TObject;
                Debug.Assert(dragSourceObject != null);
                Debug.Assert(dragSourceContainer != null);

                ItemsControl dropContainer = sender as ItemsControl;
                TObject dropTarget = e.Source as TObject;
                if(dropTarget == null)
                    dropTarget = DragDropFramework.Utilities.FindParentControlExcludingMe<TObject>(e.Source as DependencyObject);

                if(dropContainer != null) {
                    if(bDrop) {
                        dataProvider.Unparent();
                        Button button;
#if REUSE_SAME_BUTTON //|| true
                        button = dragSourceObject as Button;
#else
                        Button oldButton = dragSourceObject as Button;
                        button = new Button();
                        button.Content = DragDropFramework.Utilities.CloneElement(oldButton.Content);
                        button.ToolTip = oldButton.ToolTip;
#endif
                        if(dropTarget == null)
                            dropContainer.Items.Add(button);
                        else
                            dropContainer.Items.Insert(dropContainer.Items.IndexOf(dropTarget), button);
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
