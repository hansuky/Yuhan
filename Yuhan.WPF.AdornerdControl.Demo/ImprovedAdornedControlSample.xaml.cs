using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using System.Diagnostics;
using System.Windows.Controls.Primitives;

namespace Yuhan.WPF.AdornerdControl.Demo
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class ImprovedAdornedControlSample : Window
    {
        public ImprovedAdornedControlSample()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
        }

        private void closeButton_Click(object sender, RoutedEventArgs e)
        {
            canvas.Children.Remove(adornedControl);
        }

        private void Thumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            Canvas.SetLeft(adornedControl, Canvas.GetLeft(adornedControl) + e.HorizontalChange);
            Canvas.SetTop(adornedControl, Canvas.GetTop(adornedControl) + e.VerticalChange);
        }
    }
}
