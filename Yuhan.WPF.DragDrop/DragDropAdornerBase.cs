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

namespace Yuhan.WPF.DragDrop
{
    /// <summary>
    /// Interaction logic for DragDropAdornerBase.xaml
    /// </summary>
    public class DragDropAdornerBase : UserControl
    {
        public DragDropAdornerBase()
        {
            ScaleTransform scale = new ScaleTransform(1f,1f);
            SkewTransform skew = new SkewTransform(0f,0f);
            RotateTransform rotate = new RotateTransform(0f);
            TranslateTransform trans = new TranslateTransform(0f,0f);
            TransformGroup transGroup = new TransformGroup();
            transGroup.Children.Add(scale);
            transGroup.Children.Add(skew);
            transGroup.Children.Add(rotate);
            transGroup.Children.Add(trans);
            
            this.RenderTransform = transGroup;
        }

        public DropState AdornerDropState
        {
            get { return (DropState)GetValue(AdornerDropStateProperty); }
            set { SetValue(AdornerDropStateProperty, value); }
        }

        // Using a DependencyProperty as the backing store for DropState.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AdornerDropStateProperty =
            DependencyProperty.Register("AdornerDropState", typeof(DropState), typeof(DragDropAdornerBase), new UIPropertyMetadata(DropStateChanged));

        public static void DropStateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DragDropAdornerBase myclass = (DragDropAdornerBase)d;
            myclass.StateChangedHandler(d,e);
        }

        public virtual void StateChangedHandler(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
        }
    }

    public enum DropState
    {
        CanDrop = 1,
        CannotDrop = 2
    }
}
