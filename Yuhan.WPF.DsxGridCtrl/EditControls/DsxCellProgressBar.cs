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
    public class DsxCellProgressBar : ProgressBar
    {
        #region ctors

        static DsxCellProgressBar()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DsxCellProgressBar), new FrameworkPropertyMetadata((typeof(DsxCellProgressBar))));
        }

        public DsxCellProgressBar()
        {
            this.SmallChange = 1.0; //  TODO RangeStep
        }
        #endregion

        #region DP - Text

        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(DsxCellProgressBar), new PropertyMetadata(null));

        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }
        #endregion

        #region DP - TextAlignment

        public static readonly DependencyProperty TextAlignmentProperty =
            DependencyProperty.Register("TextAlignment", typeof(TextAlignment), typeof(DsxCellProgressBar), new PropertyMetadata(TextAlignment.Left));

        public TextAlignment TextAlignment
        {
            get { return (TextAlignment)GetValue(TextAlignmentProperty); }
            set { SetValue(TextAlignmentProperty, value); }
        }
        #endregion

        #region DP - ContentBackground

        public static readonly DependencyProperty ContentBackgroundProperty =
            DependencyProperty.Register("ContentBackground", typeof(Brush), typeof(DsxCellProgressBar), new PropertyMetadata(null));

        public Brush ContentBackground
        {
            get { return (Brush)GetValue(ContentBackgroundProperty); }
            set { SetValue(ContentBackgroundProperty, value); }
        }
        #endregion
        
    }
}
