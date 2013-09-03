using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Yuhan.WPF.MenuKiller
{
    /// <summary>
    /// Assumptions:
    ///     o The child elements are on a circle. The children will be centered respective to a point they provide (MenuKillerItems), otherwise to their center.
    ///     o There is a multitude of options on how the panel is supposed to behave, but within the arrange and measure methods, this information is compressed into very few variables.
    /// </summary>
    public partial class CircularPanel : Panel, ICustomAlignedControl
    {
        public delegate void OnChildArranged(object sender, UIElement child, double angle);
        public event OnChildArranged ChildArranged;

        public CircularPanel()
        {
        }

        #region Dependency Properties
        #region StartAngle DP
        [Category("Arrange")]
        [Description(@"Sets the angle in degrees where the first item will 
            be placed. 0 degrees points to the top.")]
        public double StartAngle
        {
            get { return (double)GetValue(StartAngleProperty); }
            set { SetValue(StartAngleProperty, value); }
        }

        public static readonly DependencyProperty StartAngleProperty =
            DependencyProperty.Register("StartAngle", typeof(double), typeof(CircularPanel), 
            new FrameworkPropertyMetadata(45d, FrameworkPropertyMetadataOptions.AffectsArrange));
        #endregion

        #region EndAngle DP
        [Category("Arrange")]
        [Description(@"Sets the angle in degrees where the last item will be placed. 0 degrees 
            points to the top. Note that this will be ignored if AngleSpacing is specified.")]
        [TypeConverterAttribute(typeof(DoubleAutoConverter))]
        public double EndAngle
        {
            get { return (double)GetValue(EndAngleProperty); }
            set { SetValue(EndAngleProperty, value); }
        }

        public static readonly DependencyProperty EndAngleProperty =
            DependencyProperty.Register("EndAngle", typeof(double), typeof(CircularPanel), 
            new FrameworkPropertyMetadata(Double.NaN, FrameworkPropertyMetadataOptions.AffectsArrange));
        #endregion

        #region Radius DP
        [Category("Arrange")]
        [Description(@"Sets the radius of the circle where children will be positioned on. 
            Note that the resulting panel might be siginificantly larger.")]
        public double Radius
        {
            get { return (double)GetValue(RadiusProperty); }
            set { SetValue(RadiusProperty, value); }
        }

        public static readonly DependencyProperty RadiusProperty =
            DependencyProperty.Register("Radius", typeof(double), typeof(CircularPanel), 
                new FrameworkPropertyMetadata(45d, FrameworkPropertyMetadataOptions.AffectsMeasure));
        #endregion

        #region AngleSpacing DP
        [Category("Arrange")]
        [Description(@"Sets the number of degress between two items. When set to 'auto', children 
            will be arranged evenly distributed between StartAngle and EndAngle.")]
        [TypeConverterAttribute(typeof(DoubleAutoConverter))]
        public double AngleSpacing
        {
            get { return (double)GetValue(AngleSpacingProperty); }
            set { SetValue(AngleSpacingProperty, value); }
        }

        public static readonly DependencyProperty AngleSpacingProperty =
            DependencyProperty.Register("AngleSpacing", typeof(double), typeof(CircularPanel), 
                new FrameworkPropertyMetadata(45d, FrameworkPropertyMetadataOptions.AffectsArrange));
        #endregion
        #endregion

        #region ICustomAlignedControl Implementation
        /// <summary>
        /// Retrieves the center of the circle, relative to the bounds of this objects.
        /// Since the Circle is always at (0, 0) in the local coordinate system, the circle center 
        /// in the relative coordinate system is just the upper-left corner.
        /// </summary>
        public Point AlignReferencePoint
        {
            get
            {
                if (dXMin != Double.MaxValue && dYMin != Double.MaxValue)
                {
                    return new Point(-dXMin, -dYMin);
                }

                return new Point();
            }
        }
        #endregion

        #region Member Variables
        static double dDegToRad = Math.PI / 180.0;
        
        double dRenderRadius = 0;
        double dRenderAngleSpacing = 0;

        double dXMax = Double.MinValue;
        double dYMax = Double.MinValue;

        double dXMin = Double.MaxValue;
        double dYMin = Double.MaxValue;
        #endregion

        void RecalcParams()
        {
            dRenderRadius = Radius;
            UpdateRenderAngleSpacing();
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            Size size = new Size(Double.PositiveInfinity, Double.PositiveInfinity);
            Size resultSize = new Size();

            if (this.Children == null || this.Children.Count == 0)
            {
                return resultSize;
            }

            RecalcParams();

            int iCurrentChildIndex = 0;

            dXMax = Double.MinValue;
            dYMax = Double.MinValue;

            dXMin = Double.MaxValue;
            dYMin = Double.MaxValue;

            foreach (UIElement child in Children)
            {
                double dAngle = ((double)(iCurrentChildIndex)) * dRenderAngleSpacing + StartAngle;
                double dX =  dRenderRadius * Math.Sin(dAngle * dDegToRad);

                // We invert the Y coordinate, because the origin in controls 
                // is the upper left corner, rather than the lower left
                double dY = -dRenderRadius * Math.Cos(dAngle * dDegToRad);

                child.Measure(size);

                Point visualCenter = new Point();

                if (child is ICustomAlignedControl)
                {
                    ICustomAlignedControl mkchild = (ICustomAlignedControl)child;
                    visualCenter.X = mkchild.AlignReferencePoint.X;
                    visualCenter.Y = mkchild.AlignReferencePoint.Y;
                }
                else
                {
                    visualCenter.X = child.DesiredSize.Width * 0.5;
                    visualCenter.Y = child.DesiredSize.Height * 0.5;
                }

                dXMax = Math.Max(dXMax, dX + (child.DesiredSize.Width - visualCenter.X));
                dYMax = Math.Max(dYMax, dY + (child.DesiredSize.Height - visualCenter.Y));

                dXMin = Math.Min(dXMin, dX - (visualCenter.X));
                dYMin = Math.Min(dYMin, dY - (visualCenter.Y));
                iCurrentChildIndex++;
            }

            resultSize.Width = dXMax - dXMin;
            resultSize.Height = dYMax - dYMin;

            return resultSize;
        }

        private Vector GetChildOffset(UIElement child, double dAngle)
        {
            Vector Offset = new Vector();

            if (child is ICustomAlignedControl)
            {
                ICustomAlignedControl mkchild = (ICustomAlignedControl)child;
                Offset.X =  dRenderRadius * Math.Sin(dAngle * dDegToRad) - dXMin - mkchild.AlignReferencePoint.X;
                Offset.Y = -dRenderRadius * Math.Cos(dAngle * dDegToRad) - dYMin - mkchild.AlignReferencePoint.Y;
            }
            else
            {
                Offset.X =  dRenderRadius * Math.Sin(dAngle * dDegToRad) - dXMin - child.DesiredSize.Width * 0.5;
                Offset.Y = -dRenderRadius * Math.Cos(dAngle * dDegToRad) - dYMin - child.DesiredSize.Height * 0.5;
            }

            return Offset;
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            if (this.Children == null || this.Children.Count == 0)
            {
                return finalSize;
            }

            double maxRadius = Math.Min(finalSize.Width * 0.5, finalSize.Height * 0.5);
            
            // AutoScale, AngleSpacing
            RecalcParams();

            int iCurrentChildIndex = 0;

            foreach (UIElement child in Children)
            {
                double dAngle = ((double)(iCurrentChildIndex)) * dRenderAngleSpacing + StartAngle;

                Vector offset = GetChildOffset(child, dAngle);
                
                child.Arrange(new Rect(offset.X, offset.Y, child.DesiredSize.Width, child.DesiredSize.Height));

                if (null != ChildArranged)
                {
                    // Notify subscribers that a child has been assigned to a specific location.
                    // MenuKillerItems will use this to tell the newly positioned child item 
                    // which direction it should grow to.
                    ChildArranged(this, child, dAngle);
                }

                iCurrentChildIndex++;
            }

            return finalSize;
        }

        #if DEBUG
        protected override void OnRender(DrawingContext dc)
        {
            base.OnRender(dc);

            System.Windows.Media.Pen p = new System.Windows.Media.Pen(System.Windows.Media.Brushes.LightBlue, 2.0);

            // Rect r = LayoutInformation.GetLayoutSlot(this); // available space, includes margin
            // RenderSize // is what is available, w/o margin
            // DesiredSize // includes Margin
            
            dc.DrawRectangle(null, p, new Rect(0, 0, DesiredSize.Width - Margin.Left - Margin.Right, DesiredSize.Height - Margin.Top - Margin.Bottom));
            dc.DrawEllipse(null, p, AlignReferencePoint, dRenderRadius, dRenderRadius);
            dc.DrawEllipse(null, p, AlignReferencePoint, 1.0, 1.0);
        }
        #endif


        /// <summary>
        /// Updates the RenderAngleSpacing and handles the 'Auto' case when AngleSpacing is NaN.
        /// </summary>
        void UpdateRenderAngleSpacing()
        {
            if (null != Children && Children.Count > 0)
            {
                double dChildCount = (double)Children.Count - 1;

                if (Double.IsNaN(AngleSpacing))
                {
                    if (dChildCount >= 1)
                    {
                        if (Double.IsNaN(EndAngle))
                        {
                            // Distribute evenly across the full circle
                            dRenderAngleSpacing = (360.0 / (dChildCount + 1));
                        }
                        else
                        {
                            // Place the first on start angle, last on end angle. Note that
                            // child count is thus smaller!
                            dRenderAngleSpacing = ((EndAngle - StartAngle) / dChildCount);
                        }
                    }
                    else
                    {
                        dRenderAngleSpacing = 0.0;
                    }
                }
                else
                {
                    dRenderAngleSpacing = AngleSpacing;
                }
            }
        }
    }
}
