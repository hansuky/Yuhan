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

namespace Yuhan.WPF.DragDrop.Demo.Example2
{
    /// <summary>
    /// Interaction logic for TextDragDropAdorner.xaml
    /// </summary>
    public partial class TextDragDropAdorner : DragDropAdornerBase
    {
        public TextDragDropAdorner()
        {
            InitializeComponent();
        }

        public override void StateChangedHandler(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TextDragDropAdorner myclass = (TextDragDropAdorner)d;

            switch ((DropState)e.NewValue)
            {
                case DropState.CanDrop:
                    myclass.back.Stroke = Application.Current.Resources["canDropBrush"] as SolidColorBrush;
                    myclass.indicator.Source = Application.Current.Resources["dropIcon"] as DrawingImage;
                    break;
                case DropState.CannotDrop:
                    myclass.back.Stroke = Application.Current.Resources["solidRed"] as SolidColorBrush;
                    myclass.indicator.Source = Application.Current.Resources["noDropIcon"] as DrawingImage;
                    break;
            }
        }
    }
}
