using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Yuhan.WPF.Demo.ViewModels;

namespace Yuhan.WPF.Demo
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : Window
    {
        private MainViewModel ViewModel
        {
            get { return this.FindResource("ViewModel") as MainViewModel; }
        }

        public MainWindow()
        {
            InitializeComponent();
        }

        private void SettingBtn_Click(object sender, RoutedEventArgs e)
        {
            ShapeMenuList.Show();
            this.VisualContainer.ShowGridLines = true;
        }

        private void HideBtn_Click(object sender, RoutedEventArgs e)
        {
            ShapeMenuList.Hide();
            this.VisualContainer.ShowGridLines = false;
        }

        private void GridContentBtn_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ClearContent_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.Containers.Clear();
        }
    }
}
