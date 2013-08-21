using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Shapes;



namespace Yuhan.WPF.DragDrop.DragDropFramework
{
    public class DefaultAdorner : Adorner
    {
        private UIElement _child;
        private Point _adornerOrigin;
        private Point _adornerOffset;

        /// <summary>
        /// Create an adorner with default opacity.
        /// The created adorner must then be added to the AdornerLayer.
        /// </summary>
        /// <param name="adornedElement">Element whose AdornerLayer will be use for displaying the adorner</param>
        /// <param name="adornerElement">Element used as adorner</param>
        /// <param name="adornerOrigin">Origin offset within the adorner</param>
        public DefaultAdorner(UIElement adornedElement, UIElement adornerElement, Point adornerOrigin)
            : this(adornedElement, adornerElement, adornerOrigin, 0.3)
        {
        }

        /// <summary>
        /// Create an adorner.
        /// The created adorner must then be added to the AdornerLayer.
        /// </summary>
        /// <param name="adornedElement">Element whose AdornerLayer will be use for displaying the adorner</param>
        /// <param name="adornerElement">Element used as adorner</param>
        /// <param name="adornerOrigin">Origin offset within the adorner</param>
        /// <param name="opacity">Adorner's opacity</param>
        public DefaultAdorner(UIElement adornedElement, UIElement adornerElement, Point adornerOrigin, double opacity)
            : base(adornedElement)
        {
            Rectangle rect = new Rectangle();
            rect.Width = adornerElement.RenderSize.Width;
            rect.Height = adornerElement.RenderSize.Height;

            VisualBrush visualBrush = new VisualBrush(adornerElement);
            visualBrush.Opacity = opacity;
            visualBrush.Stretch = Stretch.None;
            rect.Fill = visualBrush;

            this._child = rect;

            this._adornerOrigin = adornerOrigin;
        }

        /// <summary>
        /// Set the position of and redraw the adorner.
        /// Call when the mouse cursor position changes.
        /// </summary>
        /// <param name="position">Adorner's new position relative to AdornerLayer origin</param>
        public void SetMousePosition(Point position) {
            this._adornerOffset.X = position.X - this._adornerOrigin.X;
            this._adornerOffset.Y = position.Y - this._adornerOrigin.Y;
            UpdatePosition();
        }

        private void UpdatePosition() {
            AdornerLayer adornerLayer = (AdornerLayer)this.Parent;
            if(adornerLayer != null) {
                adornerLayer.Update(this.AdornedElement);
            }
        }

        protected override int VisualChildrenCount { get { return 1; } }

        protected override Visual GetVisualChild(int index) {
            System.Diagnostics.Debug.Assert(index == 0, "Index must be 0, there's only one child");
            return this._child;
        }

        protected override Size MeasureOverride(Size finalSize) {
            this._child.Measure(finalSize);
            return this._child.DesiredSize;
        }

        protected override Size ArrangeOverride(Size finalSize) {
            this._child.Arrange(new Rect(finalSize));
            return finalSize;
        }

        public override GeneralTransform GetDesiredTransform(GeneralTransform transform) {
            GeneralTransformGroup newTransform = new GeneralTransformGroup();
            newTransform.Children.Add(base.GetDesiredTransform(transform));
            newTransform.Children.Add(new TranslateTransform(this._adornerOffset.X, this._adornerOffset.Y));
            return newTransform;
        }
    }
}
