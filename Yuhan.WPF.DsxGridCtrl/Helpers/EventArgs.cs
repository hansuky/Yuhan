using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Yuhan.WPF.DsxGridCtrl
{
    public class EventArgs<TValue> : EventArgs
    {
        public TValue Value { get; set; }
        public EventArgs(TValue value)
        {
            this.Value = value;
        }
    }
}
