using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System;
using System.Windows.Threading;

namespace Yuhan.WPF.DsxGridCtrl
{
	public class DsxFilterStyle : Border
    {
        #region ctors

        public DsxFilterStyle()
        {
        }
        #endregion

        #region members / properties

        #endregion

        #region DP - CriteriaBackground

        public static readonly DependencyProperty CriteriaBackgroundProperty =
            DependencyProperty.Register("CriteriaBackground", typeof(Brush), typeof(DsxFilterStyle), new PropertyMetadata(null));

        public Brush CriteriaBackground
        {
            get { return (Brush)GetValue(CriteriaBackgroundProperty); }
            set { SetValue(CriteriaBackgroundProperty, value); }
        }
        #endregion
    }
}
