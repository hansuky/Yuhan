using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Yuhan.WPF.VisualContainer
{
    interface IShapeContainer
    {
        Double Width { get; set; }
        Double Height { get; set; }
    }

    interface ICanvasCoordinator
    {
        Double X { get; set; }
        Double Y { get; set; }
    }

    interface IGridCoordinator
    {
        int Row { get; set; }
        int Cell { get; set; }
    }
}
