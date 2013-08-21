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
    public class DsxCellSlider : DsxCellProgressBar
    {
        #region ctors

        static DsxCellSlider()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DsxCellSlider), new FrameworkPropertyMetadata((typeof(DsxCellSlider))));

            ResourceDictionary _resDictionary           = new ResourceDictionary();
                               _resDictionary.Source    = new Uri("/Yuhan.WPF.DsxGridCtrl;component/Themes/DsxCellSlider.xaml", UriKind.Relative);

            sThumbStyle = _resDictionary["dsxHorizontalSliderThumbStyle"] as Style;
        }

        public DsxCellSlider()
        {
            this.Loaded += OnLoaded;
        }
        #endregion

        #region EventConsumer - OnLoaded

        void OnLoaded(object sender, RoutedEventArgs e)
        {
            //  since the element is not a known TemplateChild,
            //  we have to modify after all is loaded

            Thumb  _thumb = ElementHelper.FindVisualChild<Thumb>(this, "Thumb");

            if (_thumb != null)
            {
                _thumb.FocusVisualStyle = null;
                //_thumb.Style = sThumbStyle;
            }
        }
        #endregion

        #region members / properties

        private static Style sThumbStyle { get; set; }
        #endregion
    }
}
