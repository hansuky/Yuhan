using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Yuhan.WPF.MenuKiller
{
    public class ReferenceAlignPanel : Panel, ICustomAlignedControl
    {
        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);

            AdjustAlignReferencePoint(new Size());

            if (!Double.IsNaN(InitialAlignReferencePoint.X) || !Double.IsNaN(InitialAlignReferencePoint.Y))
                AllowRealign = false;
        }

        #region Dependency Properties
        public static readonly DependencyProperty VerticalReferencePointAlignmentProperty =
            DependencyProperty.Register(
                "VerticalReferencePointAlignment",
                typeof(VerticalAlignment),
                typeof(ReferenceAlignPanel), 
                new FrameworkPropertyMetadata
                    (VerticalAlignment.Stretch, FrameworkPropertyMetadataOptions.AffectsArrange));

        public VerticalAlignment VerticalReferencePointAlignment
        {
            get
            {
                return (VerticalAlignment)GetValue(VerticalReferencePointAlignmentProperty);
            }
            set
            {
                SetValue(VerticalReferencePointAlignmentProperty, value);
            }
        }

        public static readonly DependencyProperty HorizontalReferencePointAlignmentProperty =
            DependencyProperty.Register(
                "HorizontalReferencePointAlignment",
                typeof(HorizontalAlignment),
                typeof(ReferenceAlignPanel), new FrameworkPropertyMetadata(HorizontalAlignment.Stretch, FrameworkPropertyMetadataOptions.AffectsArrange));

        public HorizontalAlignment HorizontalReferencePointAlignment 
        {
            get
            {
                return (HorizontalAlignment)GetValue(HorizontalReferencePointAlignmentProperty);
            }
            set
            {
                SetValue(HorizontalReferencePointAlignmentProperty, value);
            }
        }

        public static readonly DependencyProperty InitialAlignReferencePointProperty =
            DependencyProperty.Register(
                "InitialAlignReferencePoint",
                typeof(Point),
                typeof(ReferenceAlignPanel), new FrameworkPropertyMetadata(new Point(Double.NaN, Double.NaN), FrameworkPropertyMetadataOptions.AffectsParentMeasure));

        public Point InitialAlignReferencePoint
        {
            get
            {
                return (Point)GetValue(InitialAlignReferencePointProperty);
            }
            set
            {
                SetValue(InitialAlignReferencePointProperty, value);
            }
        }

        public static readonly DependencyProperty AllowRealignProperty =
            DependencyProperty.Register(
                "AllowRealign",
                typeof(bool),
                typeof(ReferenceAlignPanel), new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.AffectsMeasure));

        public bool AllowRealign
        {
            get
            {
                return (bool)GetValue(AllowRealignProperty);
            }
            set
            {
                SetValue(AllowRealignProperty, value);
            }
        }
        #endregion

        private Point _alignReferencePoint;

        public Point AlignReferencePoint
        {
            get { return _alignReferencePoint; }
            set { _alignReferencePoint = value; }
        }

        public delegate void OnAlignReferencePointChanged(object sender);

        public event OnAlignReferencePointChanged AlignReferencePointChanged;


        private void AdjustAlignReferencePoint(Size bounds)
        {
            _alignReferencePoint = InitialAlignReferencePoint;


            // This is not very straightforward. Also, putting the align reference point on a border is
            // never a good idea.
            switch (HorizontalReferencePointAlignment)
            {
                case HorizontalAlignment.Center:
                    _alignReferencePoint.X = bounds.Width * 0.5;
                    break;

                case HorizontalAlignment.Left:
                    _alignReferencePoint.X = 0;
                    break;

                case HorizontalAlignment.Right:
                    _alignReferencePoint.X = bounds.Width;
                    break;
            }

            switch (VerticalReferencePointAlignment)
            {
                case VerticalAlignment.Center:
                    _alignReferencePoint.Y = bounds.Height * 0.5;
                    break;

                case VerticalAlignment.Top:
                    _alignReferencePoint.Y = 0;
                    break;

                case VerticalAlignment.Bottom:
                    _alignReferencePoint.Y = bounds.Height;
                    break;
            }

            if (Double.IsNaN(_alignReferencePoint.X) || Double.IsInfinity(_alignReferencePoint.X))
                _alignReferencePoint.X = 0;

            if (Double.IsNaN(_alignReferencePoint.Y) || Double.IsInfinity(_alignReferencePoint.Y))
                _alignReferencePoint.Y = 0;


            if (null != AlignReferencePointChanged)
                AlignReferencePointChanged(this);
        }

        private Vector GetChildOffset(UIElement child)
        {
            Vector childDesiredOffset;

            if (child is ICustomAlignedControl)
            {
                childDesiredOffset = AlignReferencePoint - ((ICustomAlignedControl)child).AlignReferencePoint;
            }
            else
            {
                // TODO: Honor the children's align properties
                childDesiredOffset = new Vector();

                childDesiredOffset.X = _alignReferencePoint.X - child.DesiredSize.Width * 0.5;
                childDesiredOffset.Y = _alignReferencePoint.Y - child.DesiredSize.Height * 0.5;
            }

            if (AllowRealign)
            {
                // If this happens, we have to re-measure all items!
                if (childDesiredOffset.X < 0)
                {
                    _alignReferencePoint.X -= childDesiredOffset.X;
                    childDesiredOffset.X = 0;
                    realignRequired = true;
                }

                if (childDesiredOffset.Y < 0)
                {
                    _alignReferencePoint.Y -= childDesiredOffset.Y;
                    childDesiredOffset.Y = 0;
                    realignRequired = true;
                }
            }

            return childDesiredOffset;
        }

        bool realignRequired = false;

        protected override Size ArrangeOverride(Size finalSize)
        {
            int arrangeCount = 0;

            // realign should not be required, but you can't bet on it. Protect from infinite iteration.
            do
            {
                foreach (UIElement child in Children)
                {
                    if (child.IsVisible)
                    {
                        Vector childOffset = GetChildOffset(child);

                        child.Arrange(new Rect(childOffset.X, childOffset.Y, child.DesiredSize.Width, child.DesiredSize.Height));
                    }
                }

                ++arrangeCount;
            } while (realignRequired && arrangeCount < 2);

            return finalSize;
        }
        

        protected override Size MeasureOverride(Size availableSize)
        {
            Size inifiniteSize = new Size(Double.PositiveInfinity, Double.PositiveInfinity);

            AdjustAlignReferencePoint(availableSize);

            bool bMeasureNecessary = true;
            int iMaxRemeasureCount = 4;

            Size neededSize = new Size();

            // ugly and convoluted
            // A high number of remeasures will not happen - except for the vs designer, it seems. Removing 
            // this check will crash VS
            // FIXME: Clean this up or use different beh. in design mode
            for (int i = 0; bMeasureNecessary && i < iMaxRemeasureCount; ++i )
            {
                neededSize.Width = neededSize.Height = 0;

                bMeasureNecessary = false; // Assume remeasure is not needed

                Vector MinimumChildOffset = new Vector(Double.MaxValue, Double.MaxValue);

                foreach (UIElement child in Children)
                {
                    child.Measure(inifiniteSize);

                    if (child.IsVisible == false)
                    {
                        continue;
                    }

                    Vector childDesiredOffset = GetChildOffset(child);

                    MinimumChildOffset.X = Math.Min(MinimumChildOffset.X, childDesiredOffset.X);
                    MinimumChildOffset.Y = Math.Min(MinimumChildOffset.Y, childDesiredOffset.Y);

                    neededSize.Width = Math.Max(neededSize.Width, childDesiredOffset.X + child.DesiredSize.Width);
                    neededSize.Height = Math.Max(neededSize.Height, childDesiredOffset.Y + child.DesiredSize.Height);
                }
                
                if (bMeasureNecessary)
                    continue;
            
                if (AllowRealign)
                {
                    if (MinimumChildOffset.X > 0)
                        _alignReferencePoint.X -= MinimumChildOffset.X;

                    if (MinimumChildOffset.Y > 0)
                        _alignReferencePoint.Y -= MinimumChildOffset.Y;

                    if (MinimumChildOffset.X > 0 || MinimumChildOffset.Y > 0)
                    {
                        bMeasureNecessary = true;
                    }
                }
            }

            if (null != AlignReferencePointChanged)
            {
                AlignReferencePointChanged(this);
            }

            return neededSize;
        }


#if DEBUG
        protected override void OnRender(System.Windows.Media.DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);


            Rect r = new Rect(0, 0, DesiredSize.Width - Margin.Left - Margin.Right, DesiredSize.Height - Margin.Top - Margin.Bottom);
            drawingContext.DrawRectangle(null, new Pen(Brushes.Tomato, 2.0), r);

            // show the visual center
            drawingContext.DrawEllipse(Brushes.SeaGreen, null, _alignReferencePoint, 3, 3);
        }
#endif
    }
}
