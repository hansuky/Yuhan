using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using Yuhan.Common.Models;

namespace Yuhan.WPF.TreeListView.Demo
{
    public class MainViewModel : NotifyPropertyChangedBase
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


        public MainViewModel()
            : base()
        {
            this.Load();
        }

        public void Load()
        {
            Composite composite1 = new Composite()
            {
                No = "1", Name = "Composite1"
            };
            composite1.Components.Add(new Leaf() { No = "1.1", Name = "Leaf1" });
            composite1.Components.Add(new Leaf() { No = "1.2", Name = "Leaf2" });
            composite1.Components.Add(new Leaf() { No = "1.3", Name = "Leaf3" });
            Components.Add(composite1);

            Composite composite2 = new Composite()
            {
                No = "2",
                Name = "Composite2"
            };
            Composite composite2_2 = new Composite()
            {
                No = "2.1",
                Name = "Composite2.2"
            };
            composite2_2.Components.Add(new Leaf() { No = "2.1.1", Name = "Leaf1" });
            composite2_2.Components.Add(new Leaf() { No = "2.1.2", Name = "Leaf1" });
            composite2_2.Components.Add(new Leaf() { No = "2.1.3", Name = "Leaf1" });
            
            composite2.Components.Add(composite2_2);
            composite2.Components.Add(new Leaf() { No = "2.2", Name = "Leaf2" });
            composite2.Components.Add(new Leaf() { No = "2.3", Name = "Leaf3" });
            Components.Add(composite2);

            Composite composite3 = new Composite()
            {
                No = "3",
                Name = "Composite3"
            };
            Composite composite3_3 = new Composite()
            {
                No = "3.1",
                Name = "Composite3.3"
            };
            composite3_3.Components.Add(new Leaf() { No = "3.1.1", Name = "Leaf1" });
            composite3_3.Components.Add(new Leaf() { No = "3.1.2", Name = "Leaf1" });
            composite3_3.Components.Add(new Leaf() { No = "3.1.3", Name = "Leaf1" });

            composite3.Components.Add(composite3_3);
            composite3.Components.Add(new Leaf() { No = "3.2", Name = "Leaf2" });
            composite3.Components.Add(new Leaf() { No = "3.3", Name = "Leaf3" });
            Components.Add(composite3);
        }
    }
}
