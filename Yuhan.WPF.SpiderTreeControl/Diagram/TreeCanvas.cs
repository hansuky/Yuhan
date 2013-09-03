using System;
using System.Linq;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using System.Windows.Media;
using System.Windows.Input;

namespace SpiderTreeControl.Diagram
{

    /// <summary>
    /// The container where all the connected nodes
    /// should be placed
    /// </summary>
    public class TreeCanvas : Canvas
    {
        #region Ctor
        public TreeCanvas()
        {

        }
        #endregion

        #region Layout
        /// <summary>
        /// Any custom Panel must override ArrangeOverride and MeasureOverride
        /// </summary>
        protected override Size ArrangeOverride(Size arrangeSize)
        {
            foreach (UIElement element in base.InternalChildren)
            {
                double x;
                double y;
                double left = Canvas.GetLeft(element);
                double top = Canvas.GetTop(element);
                x = double.IsNaN(left) ? 0 : left;
                y = double.IsNaN(top) ? 0 : top;

                element.Arrange(new Rect(new Point(x, y), element.DesiredSize));
            }
            return arrangeSize;
        }


        protected override Size MeasureOverride(Size constraint)
        {
            Size size = new Size(double.PositiveInfinity, double.PositiveInfinity);
            foreach (UIElement element in base.InternalChildren)
            {
                element.Measure(size);
            }
            return new Size();
        }


        #endregion

        #region Render Methods
        protected override void OnRender(System.Windows.Media.DrawingContext dc)
        {
            base.OnRender(dc);
            foreach (UIElement uiElement in Children)
            {
                if (uiElement is DiagramNode)
                {
                    DiagramNode node = (DiagramNode)uiElement;

                    if (node.Visibility == Visibility.Visible)
                    {
                        if (node.DiagramParent != null && 
                            node.DiagramParent.Visibility == Visibility.Visible)
                        {
                            dc.DrawLine(new Pen(Brushes.Black, 2.0), 
                                node.Location, node.DiagramParent.Location);
                        }
                    }
                }
            }
        }
        #endregion
    }
}
