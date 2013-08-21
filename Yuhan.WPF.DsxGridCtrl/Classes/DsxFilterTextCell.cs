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
    public class DsxFilterTextCell : DsxRowCell<TextBlock>
    {
        #region ctors

        public DsxFilterTextCell()
        {
            this.InitElement(null, true);
        }
        #endregion

 
        #region DP - FilterText

        public static readonly DependencyProperty FilterTextProperty =
            DependencyProperty.Register("FilterText", typeof(string), typeof(DsxFilterTextCell), new PropertyMetadata(null, OnFilterTextChanged));

        public string FilterText
        {
            get { return (string)GetValue(FilterTextProperty); }
            set { SetValue(FilterTextProperty, value); }
        }

        private static void OnFilterTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) 
        {
            if (d == null || e.NewValue == null)
            {
                return;
            }

            DsxFilterTextCell   _context    = (DsxFilterTextCell)d;
            string              _newValue   = (string)e.NewValue;
            string              _oldValue   = (string)e.OldValue;

            if (_newValue != _oldValue)
            {
                if (_newValue.EndsWith(" : "))
                {
                    _context.Text = "";
                }
                else
                {
                    _context.Text = _newValue;
                }
            }
        }
        #endregion
    }
}
