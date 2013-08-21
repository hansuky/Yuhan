using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Yuhan.WPF.MessageBox
{
    public class ProgressMessageViewModel : MessageBoxViewModel
    {
        public ProgressMessageViewModel()
            : base()
        {
            this.Percentage = 0;
        }
    }
}
