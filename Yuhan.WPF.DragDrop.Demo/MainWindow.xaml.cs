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
using Yuhan.WPF.DragDrop.Demo.Example2;

namespace Yuhan.WPF.DragDrop.Demo
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            //example 2
            ExampleImage obj = new ExampleImage("pearl jam", "/Example2/album.jpg");
            this.DataContext = obj;
            DragDropHelper.ItemDropped += new EventHandler<DragDropEventArgs>(DragDropHelper_ItemDropped);
        }

        void DragDropHelper_ItemDropped(object sender, DragDropEventArgs e)
        {

        }
    }
}
