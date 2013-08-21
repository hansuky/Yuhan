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
    public class DsxFilterClear : Button
    {
        #region ctors

        static DsxFilterClear()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DsxFilterClear), new FrameworkPropertyMetadata((typeof(DsxFilterClear))));
        }

        public DsxFilterClear()
        {
        }
        #endregion
    }
}
