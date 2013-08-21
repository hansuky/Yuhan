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
using Microsoft.Windows.Themes;

namespace Yuhan.WPF.DsxGridCtrl
{
    //  Border and a Child with desired Content T

    public class DsxRowCell<T> : DsxCellBase
        where T: FrameworkElement, new()
    {
        #region members / properties

        private bool                IsTextBlock         { get; set; }
        private bool                IsBullet            { get; set; }
        private bool                IsCheckBox          { get; set; }
        private bool                IsImage             { get; set; }
        private bool                IsProgressBar       { get; set; }

        #endregion

        #region ctors

        public DsxRowCell()
        {
        }
        #endregion

        #region Method - InitElement

        internal void InitElement(DsxColumn DsxColumn, bool isDecorator)
        {
            if (this.Child != null)
            {
                return;
            }

            this.Child      = (FrameworkElement)new T();

            FrameworkElement _element = (FrameworkElement)this.Child;
//                           _element.Style = this.DsxColumn.RowCellStyle;

            this.IsDecorator = isDecorator;

            if (_element != null)
            {
                _element.Focusable              = false;
                _element.IsHitTestVisible       = true;
                _element.FocusVisualStyle       = null;
                _element.IsEnabled              = false;
                _element.HorizontalAlignment    = HorizontalAlignment.Left;
                _element.VerticalAlignment      = VerticalAlignment.Top;
                _element.FocusVisualStyle       = null;
            }

            this.IsTextBlock    = this.Child is TextBlock;
            this.IsBullet       = this.Child is BulletChrome;
            this.IsCheckBox     = this.Child is CheckBox;
            this.IsImage        = this.Child is Image;
            this.IsProgressBar  = this.Child is DsxCellProgressBar;

            if (this.IsTextBlock)
            {
                _element.Margin                 = new Thickness(0,2,0,0);
                _element.HorizontalAlignment    = HorizontalAlignment.Stretch;
                _element.VerticalAlignment      = VerticalAlignment.Stretch;
                (_element as TextBlock).Padding = new Thickness(6,0,6,0);
            }

            if (this.IsBullet)
            {
                _element.Margin                 = new Thickness(0,3,0,0);
                _element.Height                 = 13.0;
                _element.Width                  = 13.0;
                _element.Opacity                = this.IsDecorator ? 0.0 : 1.0;
            }

            if (this.IsCheckBox)
            {
                _element.IsEnabled              = true;
                _element.Margin                 = new Thickness(0,4,0,0);
                _element.Height                 = 13.0;
                _element.Width                  = 13.0;
            }

            if (this.IsImage)
            {
                _element.Margin                 = new Thickness(0,2,0,0);
                _element.Height                 = this.CellContentSize.Height;
                _element.Width                  = this.CellContentSize.Width;
            }

            if (this.IsProgressBar)
            {
                _element.HorizontalAlignment    = HorizontalAlignment.Stretch;
                _element.VerticalAlignment      = VerticalAlignment.Top;
                _element.Margin                 = new Thickness(2,2,8,1);
                _element.Height                 = this.CellContentSize.Height;
                (_element as DsxCellProgressBar).ContentBackground = this.CellContentBackground;
            }

        }
        #endregion

        #region DP - DsxColumn

        public static readonly DependencyProperty DsxColumnProperty =
            DependencyProperty.Register("DsxColumn", typeof(DsxColumn), typeof(DsxRowCell<T>), new PropertyMetadata(null, OnDsxColumnChanged));

        public DsxColumn DsxColumn
        {
            get { return (DsxColumn)GetValue(DsxColumnProperty); }
            set { SetValue(DsxColumnProperty, value); }
        }

        private static void OnDsxColumnChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) 
        {
            if (d == null || e.NewValue == null)
            {
                return;
            }

            DsxRowCell<T>    _context    = (DsxRowCell<T>)d;
            DsxColumn     _newValue   = (DsxColumn)e.NewValue;
            DsxColumn     _oldValue   = (DsxColumn)e.OldValue;

            if (_newValue != _oldValue)
            {
                _context.InitElement(_newValue, false);
            }
        }
        #endregion



        #region DP - Text

        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(DsxRowCell<T>), new PropertyMetadata(null, OnTextChanged));

        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        private static void OnTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) 
        {
            if (d == null || e.NewValue == null)
            {
                return;
            }

            DsxRowCell<T>  _context    = (DsxRowCell<T>)d;
            string              _newValue   = (string)e.NewValue;
            string              _oldValue   = (string)e.OldValue;

            if (_newValue != _oldValue)
            {
                if (_context.IsTextBlock)
                {
                    (_context.Child as TextBlock).Text = _newValue;
                }
                if (_context.IsProgressBar)
                {
                    (_context.Child as DsxCellProgressBar).Text = _newValue;
                }
            }
        }
        #endregion

        #region DP - Value

        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(decimal), typeof(DsxRowCell<T>), new PropertyMetadata(0.0M, OnValueChanged));

        public decimal Value
        {
            get { return (decimal)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        private static void OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) 
        {
            if (d == null || e.NewValue == null)
            {
                return;
            }

            DsxRowCell<T>  _context    = (DsxRowCell<T>)d;
            decimal             _newValue   = (decimal)e.NewValue;
            decimal             _oldValue   = (decimal)e.OldValue;

            if (_newValue != _oldValue)
            {
                if (_context.IsProgressBar)
                {
                    (_context.Child as DsxCellProgressBar).Value = (double)_newValue;
                }
            }
        }
        #endregion

        #region DP - IsChecked

        public static readonly DependencyProperty IsCheckedProperty =
            DependencyProperty.Register("IsChecked", typeof(bool?), typeof(DsxRowCell<T>), new PropertyMetadata(null, OnIsCheckedChanged));

        public bool? IsChecked
        {
            get { return (bool?)GetValue(IsCheckedProperty); }
            set { SetValue(IsCheckedProperty, value); }
        }

        private static void OnIsCheckedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) 
        {
            if (d == null)
            {
                return;
            }

            DsxRowCell<T>  _context    = (DsxRowCell<T>)d;
            bool?               _newValue   = (bool?)e.NewValue;
            bool?               _oldValue   = (bool?)e.OldValue;

            if (_newValue != _oldValue)
            {
                if (_context.IsBullet)
                {
                    (_context.Child as BulletChrome).IsChecked = _newValue;
                    if (_context.IsDecorator)
                    {
                        if (_newValue != null)
                        {
                            (_context.Child as BulletChrome).Opacity = 1.0;
                        }
                        else
                        {
                            (_context.Child as BulletChrome).Opacity = 0.0;
                        }
                    }
                }
                if (_context.IsCheckBox)
                {
                    (_context.Child as CheckBox).IsChecked = _newValue;
                }
            }
        }
        #endregion

        #region DP - ImgSource

        public static readonly DependencyProperty ImgSourceProperty =
            DependencyProperty.Register("ImgSource", typeof(ImageSource), typeof(DsxRowCell<T>), new PropertyMetadata(null, OnImgSourceChanged));

        public ImageSource ImgSource
        {
            get { return (ImageSource)GetValue(ImgSourceProperty); }
            set { SetValue(ImgSourceProperty, value); }
        }

        private static void OnImgSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) 
        {
            if (d == null || e.NewValue == null)
            {
                return;
            }

            DsxRowCell<T>  _context    = (DsxRowCell<T>)d;
            ImageSource         _newValue   = (ImageSource)e.NewValue;
            ImageSource         _oldValue   = (ImageSource)e.OldValue;

            if (_newValue != _oldValue && _context.IsImage)
            {
                (_context.Child as Image).Source = _newValue;
            }
        }
        #endregion


        #region DP - CellContentSize

        public static readonly DependencyProperty CellContentSizeProperty =
            DependencyProperty.Register("CellContentSize", typeof(Size), typeof(DsxRowCell<T>), new PropertyMetadata(new Size(16.0, 16.0), OnCellContentSizeChanged));

        public Size CellContentSize
        {
            get { return (Size)GetValue(CellContentSizeProperty); }
            set { SetValue(CellContentSizeProperty, value); }
        }

        private static void OnCellContentSizeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) 
        {
            if (d == null || e.NewValue == null)
            {
                return;
            }

            DsxRowCell<T>  _context    = (DsxRowCell<T>)d;
            double              _newValue   = (double)e.NewValue;
            double              _oldValue   = (double)e.OldValue;

            if (_newValue != _oldValue && _context.IsImage)
            {
                (_context.Child as Image).Height = _newValue;
                (_context.Child as Image).Width  = _newValue;
            }
        }
        #endregion

        #region DP - CellContentBackground

        public static readonly DependencyProperty CellContentBackgroundProperty =
            DependencyProperty.Register("CellContentBackground", typeof(Brush), typeof(DsxRowCell<T>), new PropertyMetadata(Brushes.Transparent, OnCellContentBackgroundChanged));

        public Brush CellContentBackground
        {
            get { return (Brush)GetValue(CellContentBackgroundProperty); }
            set { SetValue(CellContentBackgroundProperty, value); }
        }

        private static void OnCellContentBackgroundChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) 
        {
            if (d == null || e.NewValue == null)
            {
                return;
            }

            DsxRowCell<T>  _context    = (DsxRowCell<T>)d;
            Brush               _newValue   = (Brush)e.NewValue;
            Brush               _oldValue   = (Brush)e.OldValue;

            if (_newValue != _oldValue)
            {
                if (_context.IsProgressBar)
                {
                    (_context.Child as DsxCellProgressBar).ContentBackground = _newValue;
                }
            }
        }
        #endregion

        #region DP - CellTextAlignment

        public static readonly DependencyProperty CellTextAlignmentProperty =
            DependencyProperty.Register("CellTextAlignment", typeof(TextAlignment), typeof(DsxRowCell<T>), new PropertyMetadata(TextAlignment.Left, OnCellTextAlignmentChanged));

        public TextAlignment CellTextAlignment
        {
            get         { return (TextAlignment)GetValue(CellTextAlignmentProperty); }
            private set { SetValue(CellTextAlignmentProperty, value); }
        }

        private static void OnCellTextAlignmentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) 
        {
            if (d == null || e.NewValue == null)
            {
                return;
            }

            DsxRowCell<T>  _context    = (DsxRowCell<T>)d;
            TextAlignment       _newValue   = (TextAlignment)e.NewValue;
            TextAlignment       _oldValue   = (TextAlignment)e.OldValue;

            if (_newValue != _oldValue)
            {
                if (_context.IsTextBlock)
                {
                    (_context.Child as TextBlock).TextAlignment = _newValue;
                }
                if (_context.IsProgressBar)
                {
                    (_context.Child as DsxCellProgressBar).TextAlignment = _newValue;
                }
            }
        }
        #endregion


        #region DP - CellFontFamily

        public static readonly DependencyProperty CellFontFamilyProperty =
            DependencyProperty.Register("CellFontFamily", typeof(FontFamily), typeof(DsxRowCell<T>), new PropertyMetadata(null, OnCellFontFamilyChanged));

        public FontFamily CellFontFamily
        {
            get { return (FontFamily)GetValue(CellFontFamilyProperty); }
            set { SetValue(CellFontFamilyProperty, value); }
        }

        private static void OnCellFontFamilyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) 
        {
            if (d == null || e.NewValue == null)
            {
                return;
            }

            DsxRowCell<T>  _context    = (DsxRowCell<T>)d;
            FontFamily     _newValue   = (FontFamily)e.NewValue;
            FontFamily     _oldValue   = (FontFamily)e.OldValue;

            if (_newValue != _oldValue)
            {
                if (_context.IsTextBlock)
                {
                    (_context.Child as TextBlock).FontFamily = _newValue;
                }
                if (_context.IsProgressBar)
                {
                    (_context.Child as DsxCellProgressBar).FontFamily = _newValue;
                }
            }
        }
        #endregion

        #region DP - CellFontSize

        public static readonly DependencyProperty CellFontSizeProperty =
            DependencyProperty.Register("CellFontSize", typeof(double?), typeof(DsxRowCell<T>), new PropertyMetadata(null, OnCellFontSizeChanged));

        public double CellFontSize
        {
            get { return (double)GetValue(CellFontSizeProperty); }
            set { SetValue(CellFontSizeProperty, value); }
        }

        private static void OnCellFontSizeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) 
        {
            if (d == null || e.NewValue == null)
            {
                return;
            }

            DsxRowCell<T>  _context    = (DsxRowCell<T>)d;
            double?        _newValue   = (double?)e.NewValue;
            double?        _oldValue   = (double?)e.OldValue;

            if (_newValue != _oldValue && _newValue != 0.0)
            {
                if (_context.IsTextBlock)
                {
                    (_context.Child as TextBlock).FontSize = (double)_newValue;
                }
                if (_context.IsProgressBar)
                {
                    (_context.Child as DsxCellProgressBar).FontSize = (double)_newValue;
                }
            }
        }
        #endregion

        #region DP - CellFontWeight

        public static readonly DependencyProperty CellFontWeightProperty =
            DependencyProperty.Register("CellFontWeight", typeof(FontWeight?), typeof(DsxRowCell<T>), new PropertyMetadata(null, OnCellFontWeightChanged));

        public FontWeight? CellFontWeight
        {
            get { return (FontWeight?)GetValue(CellFontWeightProperty); }
            set { SetValue(CellFontWeightProperty, value); }
        }

        private static void OnCellFontWeightChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) 
        {
            if (d == null || e.NewValue == null)
            {
                return;
            }

            DsxRowCell<T>  _context    = (DsxRowCell<T>)d;
            FontWeight?    _newValue   = (FontWeight?)e.NewValue;
            FontWeight?    _oldValue   = (FontWeight?)e.OldValue;

            if (_newValue != _oldValue)
            {
                if (_context.IsTextBlock)
                {
                    (_context.Child as TextBlock).FontWeight = (FontWeight)_newValue;
                }
                if (_context.IsProgressBar)
                {
                    (_context.Child as DsxCellProgressBar).FontWeight = (FontWeight)_newValue;
                }
            }
        }

        #endregion


        #region DP - CellForeground

        public static readonly DependencyProperty CellForegroundProperty =
            DependencyProperty.Register("CellForeground", typeof(Brush), typeof(DsxRowCell<T>), new PropertyMetadata(Brushes.Transparent, OnCellForegroundChanged));

        public Brush CellForeground
        {
            get { return (Brush)GetValue(CellForegroundProperty); }
            set { SetValue(CellForegroundProperty, value); }
        }

        private static void OnCellForegroundChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) 
        {
            if (d == null || e.NewValue == null)
            {
                return;
            }

            DsxRowCell<T>  _context    = (DsxRowCell<T>)d;
            Brush               _newValue   = (Brush)e.NewValue;
            Brush               _oldValue   = (Brush)e.OldValue;

            if (_newValue != _oldValue)
            {
                if (_context.IsTextBlock)
                {
                    (_context.Child as TextBlock).Foreground = _newValue;
                }
                if (_context.IsProgressBar)
                {
                    (_context.Child as DsxCellProgressBar).Foreground = _newValue;
                }
            }
        }
        #endregion

        #region DP - CellHAlign

        public static readonly DependencyProperty CellHAlignProperty =
            DependencyProperty.Register("CellHAlign", typeof(HorizontalAlignment), typeof(DsxRowCell<T>), new PropertyMetadata(HorizontalAlignment.Left, OnCellHAlignChanged));

        public HorizontalAlignment CellHAlign
        {
            get { return (HorizontalAlignment)GetValue(CellHAlignProperty); }
            set { SetValue(CellHAlignProperty, value); }
        }

        private static void OnCellHAlignChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) 
        {
            if (d == null || e.NewValue == null)
            {
                return;
            }

            DsxRowCell<T>      _context    = (DsxRowCell<T>)d;
            HorizontalAlignment     _newValue   = (HorizontalAlignment)e.NewValue;
            HorizontalAlignment     _oldValue   = (HorizontalAlignment)e.OldValue;

            if (_newValue != _oldValue)
            {
                if (_context.IsTextBlock || _context.IsProgressBar)
                {
                    TextAlignment _textAlignment = TextAlignment.Left;

                    switch(_newValue)
                    {
                        case HorizontalAlignment.Left:      _textAlignment = TextAlignment.Left;     break;
                        case HorizontalAlignment.Right:     _textAlignment = TextAlignment.Right;    break;
                        case HorizontalAlignment.Center:    _textAlignment = TextAlignment.Center;   break;
                    }
                    _context.CellTextAlignment = _textAlignment;
                }
                else if (_context.Child != null)
                {
                    (_context.Child as FrameworkElement).HorizontalAlignment = _newValue;
                }
            }
        }
        #endregion

        #region DP - CanGrow

        public static readonly DependencyProperty CanGrowProperty =
            DependencyProperty.Register("CanGrow", typeof(bool), typeof(DsxRowCell<T>), new PropertyMetadata(false, OnCanGrowChanged));

        public bool CanGrow
        {
            get { return (bool)GetValue(CanGrowProperty); }
            set { SetValue(CanGrowProperty, value); }
        }

        private static void OnCanGrowChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) 
        {
            if (d == null || e.NewValue == null)
            {
                return;
            }

            DsxRowCell<T>  _context    = (DsxRowCell<T>)d;
            bool                _newValue   = (bool)e.NewValue;
            bool                _oldValue   = (bool)e.OldValue;

            if (_newValue != _oldValue)
            {
                if (_context.IsTextBlock)
                {
                    (_context.Child as TextBlock).TextWrapping = _newValue ? TextWrapping.Wrap : TextWrapping.NoWrap;
                    (_context.Child as TextBlock).UpdateLayout();
                }
            }
        }
        #endregion

        #region DP - CellRangeMin

        public static readonly DependencyProperty CellRangeMinProperty =
            DependencyProperty.Register("CellRangeMin", typeof(double), typeof(DsxRowCell<T>), new PropertyMetadata(0.0, OnCellRangeMinChanged));

        public double CellRangeMin
        {
            get { return (double)GetValue(CellRangeMinProperty); }
            set { SetValue(CellRangeMinProperty, value); }
        }

        private static void OnCellRangeMinChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) 
        {
            if (d == null || e.NewValue == null)
            {
                return;
            }

            DsxRowCell<T> _context    = (DsxRowCell<T>)d;
            double             _newValue   = (double)e.NewValue;
            double             _oldValue   = (double)e.OldValue;

            if (_newValue != _oldValue)
            {
                if (_context.IsProgressBar)
                {
                    (_context.Child as DsxCellProgressBar).Minimum = _newValue;
                }
            }
        }
        #endregion

        #region DP - CellRangeMax

        public static readonly DependencyProperty CellRangeMaxProperty =
            DependencyProperty.Register("CellRangeMax", typeof(double), typeof(DsxRowCell<T>), new PropertyMetadata(0.0, OnCellRangeMaxChanged));

        public double CellRangeMax
        {
            get { return (double)GetValue(CellRangeMaxProperty); }
            set { SetValue(CellRangeMaxProperty, value); }
        }

        private static void OnCellRangeMaxChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) 
        {
            if (d == null || e.NewValue == null)
            {
                return;
            }

            DsxRowCell<T> _context    = (DsxRowCell<T>)d;
            double             _newValue   = (double)e.NewValue;
            double             _oldValue   = (double)e.OldValue;

            if (_newValue != _oldValue)
            {
                if (_context.IsProgressBar)
                {
                    (_context.Child as DsxCellProgressBar).Maximum = _newValue;
                }
            }
        }
        #endregion

    }
}
