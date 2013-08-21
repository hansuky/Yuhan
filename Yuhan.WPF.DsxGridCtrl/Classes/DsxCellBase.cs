using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Controls.Primitives;
using System.Collections;
using System.Timers;
using System.Windows.Threading;

namespace Yuhan.WPF.DsxGridCtrl
{
    public class DsxCellBase : Border
    {
        #region members / properties

        private DrawingVisual   VBorder         { get; set; }
        private Thickness       m_defaultMargin = new Thickness(-6,0,-6,0);     //  correct default Padding of a ListView cell (6,0,6,0)
        private Thickness       m_vBorderMargin = new Thickness(-6,0,-5,0);
        private CornerRadius    m_defaultRadius = new CornerRadius(0);
        private CornerRadius    m_focusedRadius = new CornerRadius(2);
        private long            m_lastTicks     = 0;
        #endregion

        #region ctors

        public DsxCellBase() : base()
        {
            this.HorizontalAlignment    = HorizontalAlignment.Stretch;
            this.VerticalAlignment      = VerticalAlignment  .Stretch;
            this.Focusable              = true;
            this.FocusVisualStyle       = null;
            this.Margin                 = m_defaultMargin; 
            this.CornerRadius           = m_defaultRadius;
            this.BorderThickness        = new Thickness(2);
            this.BorderBrush            = Brushes.Transparent;

            this.Loaded                         += OnLoaded;
            this.IsKeyboardFocusWithinChanged   += OnIsKeyboardFocusWithinChanged;
        }
        #endregion


        #region EventConsumer - OnLoaded

        void OnLoaded(object sender, RoutedEventArgs e)
        {
            this.VBorder = CreateVBorderVisual();

            if (this.HeightTracker == null)
            {
                Binding _bindingHeightTracker = new Binding 
                                                {
                                                    Mode            = BindingMode.OneTime,
                                                    Path            = new PropertyPath(DsxDataGrid.HeightTrackerProperty),
                                                    RelativeSource  = new RelativeSource(RelativeSourceMode.FindAncestor, typeof(DsxDataGrid), 1)
                                                };

                this.SetBinding(DsxCellBase.HeightTrackerProperty,          _bindingHeightTracker);
            }
            if (this.CellVBorderIsVisible == null)
            {
                Binding _bindingCellVBorder = new Binding 
                                                {
                                                    Mode            = BindingMode.OneTime,
                                                    Path            = new PropertyPath(DsxDataGrid.VerticalGridLinesIsVisibleProperty),
                                                    RelativeSource  = new RelativeSource(RelativeSourceMode.FindAncestor, typeof(DsxDataGrid), 1)
                                                };
                Binding _bindingVBorderBrush = new Binding 
                                                {
                                                    Mode            = BindingMode.OneTime,
                                                    Path            = new PropertyPath(DsxDataGrid.VerticalGridLinesBrushProperty),
                                                    RelativeSource  = new RelativeSource(RelativeSourceMode.FindAncestor, typeof(DsxDataGrid), 1)
                                                };

                Binding _bindingViewBorderBrush = new Binding 
                                                {
                                                    Mode            = BindingMode.OneTime,
                                                    Path            = new PropertyPath(DsxDataGrid.CellAdornerViewBorderBrushProperty),
                                                    RelativeSource  = new RelativeSource(RelativeSourceMode.FindAncestor, typeof(DsxDataGrid), 1)
                                                };

                Binding _bindingEditableBorderBrush = new Binding 
                                                {
                                                    Mode            = BindingMode.OneTime,
                                                    Path            = new PropertyPath(DsxDataGrid.CellAdornerEditableBorderBrushProperty),
                                                    RelativeSource  = new RelativeSource(RelativeSourceMode.FindAncestor, typeof(DsxDataGrid), 1)
                                                };

                this.SetBinding(DsxCellBase.CellVBorderIsVisibleProperty,   _bindingCellVBorder);
                this.SetBinding(DsxCellBase.CellVBorderBrushProperty,       _bindingVBorderBrush);
                this.SetBinding(DsxCellBase.CellViewBorderBrushProperty,    _bindingViewBorderBrush);
                this.SetBinding(DsxCellBase.CellEditableBorderBrushProperty,_bindingEditableBorderBrush);
            }

            if (this.Child != null && this.Column == null)
            {
                //  Celltemplate
                FrameworkElement    _childCtrl = (FrameworkElement)this.Child;
                                    _childCtrl.HorizontalAlignment = HorizontalAlignment.Stretch;
                                    _childCtrl.VerticalAlignment   = VerticalAlignment.Stretch;

                AdjustFocusableStyle(_childCtrl);
            }
        }
        #endregion

        #region EventConsumer - OnIsKeyboardFocusWithinChanged

        void OnIsKeyboardFocusWithinChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (this.Column == null)
            {
                return;
            }

            if ( (bool)e.NewValue )
            {
                this.CornerRadius = m_focusedRadius;
                this.BorderBrush  = this.Column.IsEditable ? this.CellViewBorderBrush : this.CellEditableBorderBrush;
                this.PreviewMouseLeftButtonDown += OnPreviewMouseLeftButtonDown;
            }
            else
            {
                this.BorderBrush  = this.IsRowSelected ? Brushes.Transparent : this.CellBackground;
                this.CornerRadius = m_defaultRadius;

                m_lastTicks = 0;
                this.PreviewMouseLeftButtonDown -= OnPreviewMouseLeftButtonDown;
            }
        }
        #endregion

        #region EventConsumer - OnPreviewMouseLeftButtonDown

        void OnPreviewMouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (this.Column == null)
            {
                return;
            }

            if (this.Column.ViewType == EViewType.CheckBox )
            {
                e.Handled = true;
                return;
            }

            if (this.Column.EditType == EEditType.CellTemplate && this.IsEditMode)
            {
                e.Handled = false;
                return;
            }


            if (this.Column.CellContentIsClickable)
            {
               //   let the click trough on the first button found

               Button _button = ElementHelper.FindVisualChild<Button>(this);

               if (_button != null)
               {
                   Point            _point      = e.GetPosition((UIElement)sender);
                   HitTestResult    _hitResult  = VisualTreeHelper.HitTest(this, _point);
                   if (_hitResult != null)
                   {
                      Visual   _hitElement = ElementHelper.FindVisualChild<Visual>(_button);

                      e.Handled = !(_hitElement == _hitResult.VisualHit);
                   }

                   // this way we could raise the button click event

                   //_button.RaiseEvent( new RoutedEventArgs(Button.ClickEvent));

                   //ButtonAutomationPeer _peerButton = new ButtonAutomationPeer(_button);
                   //IInvokeProvider      _invokeProv = _peerButton.GetPattern(PatternInterface.Invoke) as IInvokeProvider;
                   //                     _invokeProv.Invoke();
               }
            }
            else
            {
                e.Handled = true;

                if (this.Column.IsEditable)
                {
                    long _ticksPassed = DateTime.Now.Ticks - m_lastTicks;
                    if (_ticksPassed < 50000000)
                    {
                        m_lastTicks = 0;
                        this.Column.DataGrid.SetEditCell (this.Column, (FrameworkElement)this);
                        return;
                    }
                }
                m_lastTicks = DateTime.Now.Ticks;
            }
        }
        #endregion

        #region Method - CreateVBorderVisual

        private DrawingVisual CreateVBorderVisual()
        {
            return CreateVBorderVisual( new Size(this.ActualWidth, this.ActualHeight) );
        }

        private DrawingVisual CreateVBorderVisual(Size elementSize)
        {
            if (this.IsDecorator || this.CellVBorderIsVisible==null || !(bool)this.CellVBorderIsVisible)
            {
                return null;
            }

            if (this.VBorder != null)
            {
                RemoveVisualChild (this.VBorder);
                RemoveLogicalChild(this.VBorder);
            }
            DrawingVisual _vBorder = new DrawingVisual();

            using (DrawingContext _drawContext = _vBorder.RenderOpen() )
            {
                Rect _rect = new Rect(elementSize.Width, 0, 1, elementSize.Height);
                _drawContext.DrawRectangle(this.CellVBorderBrush, null, _rect);
            }
            AddVisualChild (_vBorder);
            AddLogicalChild(_vBorder);

            return _vBorder;
        }
        #endregion

        #region Method - PreventFocusable

        private void AdjustFocusableStyle(DependencyObject element)
        {
            if (element == null)
            {
                return;
            }
            if (element is FrameworkElement)
            {
                (element as FrameworkElement).FocusVisualStyle = null;
            }

            int _childrenCount  = VisualTreeHelper.GetChildrenCount(element);  

            for (int i = 0; i < _childrenCount; i++)  
            {    
                var _child = VisualTreeHelper.GetChild(element, i);
                AdjustFocusableStyle(_child);
            }  
        }

        #endregion

        #region Method - SetCellEditFocus

        internal bool SetCellEditFocus(DependencyObject element)
        {
            if (element == null)
            {
                return false;
            }

            FrameworkElement _element = element as FrameworkElement;

            if (_element != null && _element.Focusable)
            {
                _element.Focus();
                if (_element is TextBoxBase)
                {
                    (_element as TextBoxBase).SelectAll();
                }
                return true;
            }

            int _childrenCount  = VisualTreeHelper.GetChildrenCount(element);  

            for (int i = 0; i < _childrenCount; i++)  
            {    
                var _child = VisualTreeHelper.GetChild(element, i);
                if (SetCellEditFocus(_child))
                {
                    return true;
                }
            }
            return false;
        }
        #endregion


        #region members / properties

        private double    LastInternalHeight   { get; set; }
        #endregion


        #region Override - VisualChildrenCount

        protected override int VisualChildrenCount
        {
            //  the child of the border and the drawingvisual
            get { return 2; }
        }
        #endregion

        #region Override - GetVisualChild

        protected override Visual GetVisualChild(int index)
        {
            switch(index)
            {
                case 0: return this.Child   as Visual;
                case 1: return this.VBorder as Visual;
            }
            return null;
        }
        #endregion

        #region Override - MeasureOverride
        //  What size do you want to be? 

        protected override Size MeasureOverride(Size availableSize)
        {
            Size resultSize = new Size(0,0);

            //  this Panel has one Child only
            if (this.Child != null)
            {
                FrameworkElement    _child      = this.Child as FrameworkElement;

                _child.Measure(availableSize);

                //resultSize.Width  = Math.Max(resultSize.Width,  _child.DesiredSize.Width );
                //resultSize.Height = Math.Max(resultSize.Height, _child.DesiredSize.Height);
                resultSize.Width  = _child.DesiredSize.Width;
                resultSize.Height = _child.DesiredSize.Height;

                if (this.HeightTracker != null)
                {
                    var _listItem = this.DataContext;

                    resultSize.Height = this.HeightTracker.TrackItemHeight(_listItem, _child.DesiredSize.Height);

                    //  maybe we made decreased the needed height
                    if (this.LastInternalHeight > 0.0 
                        && this.LastInternalHeight == resultSize.Height 
                        && _child.DesiredSize.Height < this.LastInternalHeight)
                    {
                        this.HeightTracker.ResetItemHeight(_listItem, _child.DesiredSize.Height);
                        resultSize.Height = this.HeightTracker.TrackItemHeight(_listItem, _child.DesiredSize.Height);
                        if (resultSize.Height == _child.DesiredSize.Height || resultSize.Height == this.Column.DataGrid.ItemMinHeight)
                        {
                            this.LastInternalHeight = _child.DesiredSize.Height;
                        }
                    }
                    else
                    {
                        //  we track only if we have max height
                        if (resultSize.Height == _child.DesiredSize.Height 
                            || (this.Column != null && resultSize.Height == this.Column.DataGrid.ItemMaxHeight))
                        {
                            this.LastInternalHeight = resultSize.Height;
                        }
                        else
                        {
                            this.LastInternalHeight = 0.0;
                        }
                    }
                }
            }

            resultSize.Width  = double.IsPositiveInfinity(availableSize.Width ) ? resultSize.Width  : availableSize.Width;
            resultSize.Height = double.IsPositiveInfinity(availableSize.Height) ? resultSize.Height : availableSize.Height;

            if (resultSize.Height == 0.0)
            {
                return resultSize;
            }

            return resultSize;
        }
        #endregion

        #region Override - ArrangeOverride
        //  Here’s what size you get to be

        protected override Size ArrangeOverride(Size finalSize)
        {
            //  this Panel has one Child only
            if (this.Child != null)
            {
                FrameworkElement    _child      = this.Child as FrameworkElement;

                double              _offsetLeft  = 0.0;
                double              _offsetTop   = 0.0;
                double              _finalWidth  = _child.DesiredSize.Width;
                double              _finalHeight = _child.DesiredSize.Height;

                switch(_child.HorizontalAlignment)
                {
                    case System.Windows.HorizontalAlignment.Left:       _finalWidth  =  finalSize.Width;                                        break;
                    case System.Windows.HorizontalAlignment.Right:      _offsetLeft  =  finalSize.Width - _child.DesiredSize.Width;             break;
                    case System.Windows.HorizontalAlignment.Center:     _offsetLeft  = (finalSize.Width - _child.DesiredSize.Width) * 0.5;      break;
                    case System.Windows.HorizontalAlignment.Stretch:    _finalWidth  =  finalSize.Width;                                        break;
                }
                switch(_child.VerticalAlignment)
                {
                    case System.Windows.VerticalAlignment.Top:          _finalHeight =  finalSize.Height;                                       break;
                    case System.Windows.VerticalAlignment.Bottom:       _offsetTop   =  finalSize.Height - _child.DesiredSize.Height;           break;
                    case System.Windows.VerticalAlignment.Center:       _offsetTop   = (finalSize.Height - _child.DesiredSize.Height) * 0.5;    break;
                    case System.Windows.VerticalAlignment.Stretch:      _finalHeight =  finalSize.Height;                                       break;
                }
                _child.Arrange(new Rect(_offsetLeft, _offsetTop, _finalWidth, _finalHeight));
            }

            this.VBorder = CreateVBorderVisual(finalSize);

            return finalSize;
        }
        #endregion


        #region DP - HeightTracker

        public static readonly DependencyProperty HeightTrackerProperty =
            DependencyProperty.Register("HeightTracker", typeof(DsxHeightTracker), typeof(DsxCellBase), new PropertyMetadata(null));

        public DsxHeightTracker HeightTracker
        {
            get { return (DsxHeightTracker)GetValue(HeightTrackerProperty); }
            set { SetValue(HeightTrackerProperty, value); }
        }
        #endregion

        #region DP - Column

        public static readonly DependencyProperty ColumnProperty =
            DependencyProperty.Register("Column", typeof(DsxColumn), typeof(DsxCellBase), new PropertyMetadata(null));

        public DsxColumn Column
        {
            get { return (DsxColumn)GetValue(ColumnProperty); }
            set { SetValue(ColumnProperty, value); }
        }
        #endregion



        #region DP - CellVBorderIsVisible

        public static readonly DependencyProperty CellVBorderIsVisibleProperty =
            DependencyProperty.Register("CellVBorderIsVisible", typeof(bool?), typeof(DsxCellBase), new PropertyMetadata(null, OnCellVBorderIsVisibleChanged));

        public bool? CellVBorderIsVisible
        {
            get { return (bool?)GetValue(CellVBorderIsVisibleProperty); }
            set { SetValue(CellVBorderIsVisibleProperty, value); }
        }

        private static void OnCellVBorderIsVisibleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) 
        {
            if (d == null || e.NewValue == null)
            {
                return;
            }

            DsxCellBase  _context    = (DsxCellBase)d;
            bool?             _newValue  = (bool?)e.NewValue;
            bool?             _oldValue  = (bool?)e.OldValue;

            if (_newValue != _oldValue)
            {
                _context.Margin = ((bool)_newValue) && !_context.IsDecorator ? _context.m_vBorderMargin : _context.m_defaultMargin;
                _context.CreateVBorderVisual();
            }
        }
        #endregion

        #region DP - CellVBorderBrush

        public static readonly DependencyProperty CellVBorderBrushProperty =
            DependencyProperty.Register("CellVBorderBrush", typeof(Brush), typeof(DsxCellBase), new PropertyMetadata( Brushes.DimGray) );

        public Brush CellVBorderBrush
        {
            get { return (Brush)GetValue(CellVBorderBrushProperty); }
            set { SetValue(CellVBorderBrushProperty, value); }
        }
        #endregion

        #region DP - CellViewBorderBrush

        public static readonly DependencyProperty CellViewBorderBrushProperty =
            DependencyProperty.Register("CellViewBorderBrush", typeof(Brush), typeof(DsxCellBase), new PropertyMetadata( Brushes.Navy) );

        public Brush CellViewBorderBrush
        {
            get { return (Brush)GetValue(CellViewBorderBrushProperty); }
            set { SetValue(CellViewBorderBrushProperty, value); }
        }
        #endregion

        #region DP - CellEditableBorderBrush

        public static readonly DependencyProperty CellEditableBorderBrushProperty =
            DependencyProperty.Register("CellEditableBorderBrush", typeof(Brush), typeof(DsxCellBase), new PropertyMetadata( Brushes.Navy) );

        public Brush CellEditableBorderBrush
        {
            get { return (Brush)GetValue(CellEditableBorderBrushProperty); }
            set { SetValue(CellEditableBorderBrushProperty, value); }
        }
        #endregion


        #region DP - CellBackground

        public static readonly DependencyProperty CellBackgroundProperty =
            DependencyProperty.Register("CellBackground", typeof(Brush), typeof(DsxCellBase), new PropertyMetadata( Brushes.Transparent, OnCellBackgroundChanged) );

        public Brush CellBackground
        {
            get { return (Brush)GetValue(CellBackgroundProperty); }
            set { SetValue(CellBackgroundProperty, value); }
        }

        private static void OnCellBackgroundChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) 
        {
            if (d == null || e.NewValue == null)
            {
                return;
            }

            DsxCellBase    _context    = (DsxCellBase)d;
            Brush                   _newValue   = (Brush)e.NewValue;
            Brush                   _oldValue   = (Brush)e.OldValue;

            if (_newValue != _oldValue)
            {
                _context.Background     = _newValue;
                _context.BorderBrush    = _newValue;
            }
        }
        #endregion


        #region DP - IsRowSelected

        public static readonly DependencyProperty IsRowSelectedProperty =
            DependencyProperty.Register("IsRowSelected", typeof(bool), typeof(DsxCellBase), new PropertyMetadata(false, OnIsRowSelectedChanged));

        public bool IsRowSelected
        {
            get { return (bool)GetValue(IsRowSelectedProperty); }
            set { SetValue(IsRowSelectedProperty, value); }
        }

        private static void OnIsRowSelectedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) 
        {
            if (d == null || e.NewValue == null)
            {
                return;
            }

            DsxCellBase    _context    = (DsxCellBase)d;
            bool                    _newValue   = (bool)e.NewValue;
            bool                    _oldValue   = (bool)e.OldValue;

            if (_newValue != _oldValue)
            {
                _context.Background   = (_newValue ? Brushes.Transparent : _context.CellBackground);
                _context.BorderBrush  = _context.Background;
            }
        }
        #endregion

        #region DP - IsEditMode

        public static readonly DependencyProperty IsEditModeProperty =
            DependencyProperty.Register("IsEditMode", typeof(bool), typeof(DsxCellBase), new PropertyMetadata(false));

        public bool IsEditMode
        {
            get { return (bool)GetValue(IsEditModeProperty); }
            set { SetValue(IsEditModeProperty, value); }
        }
        #endregion

        #region DP - IsDecorator

        public static readonly DependencyProperty IsDecoratorProperty =
            DependencyProperty.Register("IsDecorator", typeof(bool), typeof(DsxCellBase), new PropertyMetadata(false));

        internal bool IsDecorator
        {
            get { return (bool)GetValue(IsDecoratorProperty); }
            set { SetValue(IsDecoratorProperty, value); }
        }
        #endregion

    }
}
