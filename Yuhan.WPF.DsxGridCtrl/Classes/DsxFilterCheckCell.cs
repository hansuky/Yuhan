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
    public class DsxFilterCheckCell : DsxRowCell<BulletChrome>
    {
        #region ctors

        public DsxFilterCheckCell()
        {
            this.InitElement(null, true);
        }
        #endregion
    }
}
