using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using Yuhan.Common.Models;
using Yuhan.WPF.VisualContainer.Demo.Models.Canvas;
using Yuhan.WPF.VisualContainer.Demo.Models.Grid;

namespace Yuhan.WPF.VisualContainer.Demo
{
    public class MainViewModel : NotifyPropertyChangedBase
    {
        private ObservableCollection<CellItem> cellItems;

        public ObservableCollection<CellItem> CellItems
        {
            get
            {
                if (cellItems == null)
                    cellItems = new ObservableCollection<CellItem>();
                return cellItems;
            }
            set { ChangedPropertyChanged<ObservableCollection<CellItem>>("CellItems", ref cellItems, ref value); }
        }

        private ObservableCollection<CanvasItem> canvasItems;

        public ObservableCollection<CanvasItem> CanvasItems
        {
            get
            {
                if (canvasItems == null)
                    canvasItems = new ObservableCollection<CanvasItem>();
                return canvasItems;
            }
            set { ChangedPropertyChanged<ObservableCollection<CanvasItem>>("CanvasItems", ref canvasItems, ref value); }
        }


        public MainViewModel()
            : base()
        {
            LoadCellItems();
            LoadCanvasItems();
        }

        private void LoadCellItems()
        {
            CellItems = new ObservableCollection<CellItem>();
            CellItems.Add(new CellItem()
            {
                Column = 2,
                Row = 4
            });
            CellItems.Add(new CellItem()
            {
                Column = 4,
                Row = 4
            });
            CellItems.Add(new CellItem()
            {
                Column = 2,
                Row = 6
            });

            CellItems.Add(new CellItem()
            {
                Column = 0,
                Row = 0,
                ColumnSpan = 2,
                RowSpan = 2
            });
        }

        private void LoadCanvasItems()
        {
            CanvasItems = new ObservableCollection<CanvasItem>();
            canvasItems.Add(new CanvasItem()
            {
                X = 15,
                Y = 30,
                Width = 100,
                Height = 35
            });
            canvasItems.Add(new CanvasItem()
            {
                X = 100,
                Y = 60,
                Width = 80,
                Height = 45
            });
            canvasItems.Add(new CanvasItem()
            {
                X = 300,
                Y = 20,
                Width = 50,
                Height = 25
            });
        }
    }
}
