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
    /// This data consumer looks for an object of type string.
    /// When the item is dropped, a TextBlock is created with
    /// its text initialized to the contents of the data string.
    /// The TextBlock's origin is placed on the canvas at the
    /// point where the string was dropped.
    /// </summary>
    public class StringToCanvasTextBlock : DataConsumerBase, IDataConsumer
    {

        /// <summary>
        /// Create a string data consumer for a canvas
        /// </summary>
        /// <param name="dataFormats">A data format whose data is of type string</param>
        public StringToCanvasTextBlock(string[] dataFormats)
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
        /// Finally handle the actual drop when <code>bDrop</code> is true
        /// by creating a new TextBlock and initializing its Text property
        /// to the value of the string.  The TextBlock is placed on the
        /// canvas such that its origin is at the point when the string
        /// was dropped.
        /// </summary>
        /// <param name="bDrop">True to perform an actual drop, otherwise just return e.Effects</param>
        /// <param name="sender">DragDrop event <code>sender</code></param>
        /// <param name="e">DragDrop event arguments</param>
        private void DragOverOrDrop(bool bDrop, object sender, DragEventArgs e) {
            object dropObject = this.GetData(e);
            if((dropObject is string) && (sender is Canvas)) {
                string data = dropObject as string;
                Canvas dropContainer = sender as Canvas;

                if(dropContainer != null) {
                    if(bDrop) {
                        Point containerPoint = e.GetPosition(dropContainer);
                        TextBlock textBlock = new TextBlock();
                        textBlock.Text = dropObject.ToString();
                        dropContainer.Children.Add(textBlock);
                        Canvas.SetLeft(textBlock, containerPoint.X);
                        Canvas.SetTop(textBlock, containerPoint.Y);
                    }
                    e.Effects = DragDropEffects.Copy;
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
