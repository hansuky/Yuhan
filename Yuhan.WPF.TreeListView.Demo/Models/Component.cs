using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Yuhan.Common;
using Yuhan.Common.Models;
using Yuhan.WPF;

namespace Yuhan.WPF.TreeListView.Demo
{
    public abstract class Component : NotifyPropertyChangedBase
    {
        private String no;

        public String No
        {
            get { return no; }
            set { ChangedPropertyChanged<String>("No", ref no, ref value); }
        }

        private String name;

        public String Name
        {
            get { return name; }
            set { ChangedPropertyChanged<String>("Name", ref name, ref value); }
        }

        private DateTime date;

        public DateTime Date
        {
            get { return date; }
            set { ChangedPropertyChanged<DateTime>("Date", ref date, ref value); }
        }

        public Component() : base() { }
    }
}
