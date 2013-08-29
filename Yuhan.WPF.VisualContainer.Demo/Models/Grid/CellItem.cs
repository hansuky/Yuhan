using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Yuhan.Common.Models;

namespace Yuhan.WPF.VisualContainer.Demo.Models.Grid
{
    public class CellItem : NotifyPropertyChangedBase
    {
        private int row;

        public int Row
        {
            get { return row; }
            set { ChangedPropertyChanged<int>("Row", ref row, ref value); }
        }

        private int column;

        public int Column
        {
            get { return column; }
            set { ChangedPropertyChanged<int>("Column", ref column, ref value); }
        }

        private int rowSpan;

        public int RowSpan
        {
            get { return rowSpan; }
            set { ChangedPropertyChanged<int>("RowSpan", ref rowSpan, ref value); }
        }

        private int columnSpan;

        public int ColumnSpan
        {
            get { return columnSpan; }
            set { ChangedPropertyChanged<int>("ColumnSpan", ref columnSpan, ref value); }
        }


        public CellItem() : base() { }
    }
}
