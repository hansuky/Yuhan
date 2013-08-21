using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace Yuhan.WPF.DragDrop.DragDropFramework
{
    public class DragDropWindow : Window
    {
        // The StringBuffers are used for testing and debug
        public StringBuilder buf0 = new StringBuilder("");
        public StringBuilder buf1 = new StringBuilder("");

        public DragDropWindow()
            : base()
        {
            SnapsToDevicePixels = true;
        }
    }
}
