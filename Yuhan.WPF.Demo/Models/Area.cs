using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yuhan.WPF.Demo.Models
{
    public class Area : Container
    {
        private IEnumerable<Container> containers;

        public IEnumerable<Container> Containers
        {
            get { return containers; }
            set
            {
                containers = value;
                ChangedPropertyChanged<IEnumerable<Container>>("Containers", ref containers, ref value);
            }
        }

        private int columnSpan;

        public int ColumnSpan
        {
            get { return columnSpan; }
            set { ChangedPropertyChanged<int>("ColumnSpan", ref columnSpan, ref value); }
        }

        private int rowSpan;

        public int RowSpan
        {
            get { return rowSpan; }
            set { ChangedPropertyChanged<int>("RowSpan", ref rowSpan, ref value); }
        }

        public Area() : base() { }
    }
}
