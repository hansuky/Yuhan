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

namespace Yuhan.WPF.AdornerdControl.Demo
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void SimpleAdornedControlSample_Open(object sender, RoutedEventArgs e)
        {
            SimpleAdornedControlSample sample = new SimpleAdornedControlSample();
            sample.Show();
        }

        private void AdvancedAdornedControlSample_Open(object sender, RoutedEventArgs e)
        {
            AdvancedAdornedControlSample sample = new AdvancedAdornedControlSample();
            sample.Show();
        }

        private void ImprovedAdornedControlSample_Open(object sender, RoutedEventArgs e)
        {
            ImprovedAdornedControlSample sample = new ImprovedAdornedControlSample();
            sample.Show();
        }
    }
}
