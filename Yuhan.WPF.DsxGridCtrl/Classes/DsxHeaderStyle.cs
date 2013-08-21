using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System;
using System.Windows.Threading;

namespace Yuhan.WPF.DsxGridCtrl
{
	public class DsxHeaderStyle : Border
    {
        #region ctors

        public DsxHeaderStyle()
        {
        }
        #endregion

        #region members / properties

        #endregion

        #region DP - PressedBackground

        public static readonly DependencyProperty PressedBackgroundProperty =
            DependencyProperty.Register("PressedBackground", typeof(Brush), typeof(DsxHeaderStyle), new PropertyMetadata( null ) );

        public Brush PressedBackground
        {
            get { return (Brush)GetValue(PressedBackgroundProperty); }
            set { SetValue(PressedBackgroundProperty, value); }
        }
        #endregion


        #region DP - GripperBrush

        public static readonly DependencyProperty GripperBrushProperty =
            DependencyProperty.Register("GripperBrush", typeof(Brush), typeof(DsxHeaderStyle), new PropertyMetadata( null) );

        public Brush GripperBrush
        {
            get { return (Brush)GetValue(GripperBrushProperty); }
            set { SetValue(GripperBrushProperty, value); }
        }
        #endregion

    }
}
