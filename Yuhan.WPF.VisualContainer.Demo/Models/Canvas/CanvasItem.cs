using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Yuhan.Common.Models;

namespace Yuhan.WPF.VisualContainer.Demo.Models.Canvas
{
    public class CanvasItem : NotifyPropertyChangedBase
    {
        private Double x;

        public Double X
        {
            get { return x; }
            set { ChangedPropertyChanged<Double>("X", ref x, ref value); }
        }

        private Double y;

        public Double Y
        {
            get { return y; }
            set { ChangedPropertyChanged<Double>("Y", ref y, ref value); }
        }

        private Double width;

        public Double Width
        {
            get { return width; }
            set { ChangedPropertyChanged<Double>("Width", ref width, ref value); }
        }

        private Double height;

        public Double Height
        {
            get { return height; }
            set { ChangedPropertyChanged<Double>("Height", ref height, ref value); }
        }


        public CanvasItem() : base() { }
    }
}
