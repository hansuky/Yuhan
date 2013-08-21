using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System;
using System.Windows.Threading;

namespace Yuhan.WPF.DsxGridCtrl
{
	public abstract class DsxRowCellStyle : FrameworkElement
    {
        #region ctors

        public DsxRowCellStyle()
        {
        }
        #endregion

        #region DP - CellHAlign

        public static readonly DependencyProperty CellHAlignProperty =
            DependencyProperty.Register("CellHAlign", typeof(HorizontalAlignment), typeof(DsxRowCellStyle), new PropertyMetadata(HorizontalAlignment.Left));

        public HorizontalAlignment CellHAlign
        {
            get { return (HorizontalAlignment)GetValue(CellHAlignProperty); }
            set { SetValue(CellHAlignProperty, value); }
        }
        #endregion

        #region DP - CellBackground

        public static readonly DependencyProperty CellBackgroundProperty =
            DependencyProperty.Register("CellBackground", typeof(Brush), typeof(DsxRowCellStyle), new PropertyMetadata(null));

        public Brush CellBackground
        {
            get { return (Brush)GetValue(CellBackgroundProperty); }
            set { SetValue(CellBackgroundProperty, value); }
        }
        #endregion

        #region DP - CellForeground

        public static readonly DependencyProperty CellForegroundProperty =
            DependencyProperty.Register("CellForeground", typeof(Brush), typeof(DsxRowCellStyle), new PropertyMetadata(null));

        public Brush CellForeground
        {
            get { return (Brush)GetValue(CellForegroundProperty); }
            set { SetValue(CellForegroundProperty, value); }
        }
        #endregion


        #region DP - CellFontFamily

        public static readonly DependencyProperty CellFontFamilyProperty =
            DependencyProperty.Register("CellFontFamily", typeof(FontFamily), typeof(DsxRowCellStyle), new PropertyMetadata(null));

        public FontFamily CellFontFamily
        {
            get { return (FontFamily)GetValue(CellFontFamilyProperty); }
            set { SetValue(CellFontFamilyProperty, value); }
        }
        #endregion

        #region DP - CellFontSize

        public static readonly DependencyProperty CellFontSizeProperty =
            DependencyProperty.Register("CellFontSize", typeof(double?), typeof(DsxRowCellStyle), new PropertyMetadata(null));

        public double? CellFontSize
        {
            get { return (double?)GetValue(CellFontSizeProperty); }
            set { SetValue(CellFontSizeProperty, value); }
        }
        #endregion

        #region DP - CellFontWeight

        public static readonly DependencyProperty CellFontWeightProperty =
            DependencyProperty.Register("CellFontWeight", typeof(FontWeight?), typeof(DsxRowCellStyle), new PropertyMetadata(null));

        public FontWeight? CellFontWeight
        {
            get { return (FontWeight?)GetValue(CellFontWeightProperty); }
            set { SetValue(CellFontWeightProperty, value); }
        }
        #endregion
    }
}
