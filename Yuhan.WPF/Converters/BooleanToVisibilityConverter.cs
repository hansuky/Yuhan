using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace Yuhan.WPF.Converters
{
    public sealed class BooleanToVisibilityConverter : BooleanConverter<Visibility>
    {
        public BooleanToVisibilityConverter() :
            base(Visibility.Visible, Visibility.Collapsed) { }
    }
}
