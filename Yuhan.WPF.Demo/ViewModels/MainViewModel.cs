using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yuhan.Common.Models;
using Yuhan.WPF;
using Yuhan.WPF.Demo.Models;

namespace Yuhan.WPF.Demo.ViewModels
{
    public class MainViewModel : NotifyPropertyChangedBase
    {
        private ObservableCollection<Container> containers;

        public ObservableCollection<Container> Containers
        {
            get
            {
                if (containers == null)
                    containers = new ObservableCollection<Container>();
                return containers;
            }
            set
            {
                ChangedPropertyChanged<ObservableCollection<Container>>("Containers", ref containers, ref value);
            }
        }

        public MainViewModel() : base() { this.Load(); }

        public void Load()
        {
            this.Containers.Add(new Block() { Row = 2, Column = 3 });
            this.Containers.Add(new Block() { Row = 0, Column = 3 });
            this.Containers.Add(new Block() { Row = 1, Column = 1 });

            this.Containers.Add(new Area() { Row = 3, Column = 5, RowSpan = 2, ColumnSpan = 2 });
        }
    }
}
