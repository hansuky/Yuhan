using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yuhan.Common.Models;

namespace Yuhan.WPF.Demo.Models
{
    public class Container : NotifyPropertyChangedBase
    {
        private int column;

        public int Column
        {
            get { return column; }
            set
            {
                ChangedPropertyChanged<int>("Column", ref column, ref value);
            }
        }

        private int row;

        public int Row
        {
            get { return row; }
            set
            {
                ChangedPropertyChanged<int>("Row", ref row, ref value);
            }
        }

    }
}
