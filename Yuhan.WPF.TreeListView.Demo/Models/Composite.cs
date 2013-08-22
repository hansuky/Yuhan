using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace Yuhan.WPF.TreeListView.Demo
{
    public class Composite : Component
    {
        private ObservableCollection<Component> components;

        public ObservableCollection<Component> Components
        {
            get
            {
                if (components == null)
                    components = new ObservableCollection<Component>();
                return components;
            }
            set { ChangedPropertyChanged<ObservableCollection<Component>>("Components", ref components, ref value); }
        }

        public Composite() : base() { }
    }
}
