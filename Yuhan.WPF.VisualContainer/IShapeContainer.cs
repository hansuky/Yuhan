using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Yuhan.WPF.VisualContainer
{
    public interface IShapeContainer
    {
        Double Width { get; set; }
        Double Height { get; set; }
    }

    public interface ICanvasCoordinator
    {
        Double X { get; set; }
        Double Y { get; set; }
    }

    public interface IGridCoordinator
    {
        int Row { get; set; }
        int Cell { get; set; }
    }
}
