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
    public class DsxCellDatePicker : DatePicker
    {
        #region ctors

        static DsxCellDatePicker()
        {
            //DefaultStyleKeyProperty.OverrideMetadata(typeof(DsxCellDatePicker), new FrameworkPropertyMetadata((typeof(DsxCellDatePicker))));

            // only overiding some parts

            ResourceDictionary _resDictionary           = new ResourceDictionary();
                               _resDictionary.Source    = new Uri("/Yuhan.WPF.DsxGridCtrl;component/Themes/DsxCellDatePicker.xaml", UriKind.Relative);

            sTextBoxStyle = _resDictionary["dsxDatePickerTextBoxStyle"] as Style;
        }

        public DsxCellDatePicker()
        {
        }
        #endregion

        #region members / properties

        private static Style sTextBoxStyle { get; set; }
        #endregion

        #region Override - OnApplyTemplate

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            DatePickerTextBox _textBox = GetTemplateChild("PART_TextBox") as DatePickerTextBox;
            if (_textBox != null)
            {
                _textBox.FocusVisualStyle = null;
                _textBox.Style = sTextBoxStyle;
            }

            Button _calendarBtn = GetTemplateChild("PART_Button") as Button;
            if (_calendarBtn != null)
            {
                _calendarBtn.Width           = 20;
                _calendarBtn.Height          = 20;
                _calendarBtn.Margin          = new Thickness(0,-1,-3,-3);
                _calendarBtn.RenderTransform = new ScaleTransform(0.75, 0.75);
                _calendarBtn.HorizontalAlignment = HorizontalAlignment.Center;
            }
        }
        #endregion
    }
}
