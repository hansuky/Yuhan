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
    public class DsxCellComboBox : ComboBox
    {
        #region ctors

        static DsxCellComboBox()
        {
            //DefaultStyleKeyProperty.OverrideMetadata(typeof(DsxCellDatePicker), new FrameworkPropertyMetadata((typeof(DsxCellDatePicker))));

            // only overiding some parts

            ResourceDictionary _resDictionary           = new ResourceDictionary();
                               _resDictionary.Source    = new Uri("/Yuhan.WPF.DsxGridCtrl;component/Themes/DsxCellComboBox.xaml", UriKind.Relative);

            sTextBoxStyle = _resDictionary["dsxComboBoxEditableTextBox"] as Style;
        }

        public DsxCellComboBox()
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

            TextBox _textBox = GetTemplateChild("PART_EditableTextBox") as TextBox;
            if (_textBox != null)
            {
                _textBox.FocusVisualStyle = null;
                _textBox.Style = sTextBoxStyle;
            }
        }
        #endregion
    }
}
