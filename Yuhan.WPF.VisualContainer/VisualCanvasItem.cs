using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Yuhan.WPF.VisualContainer
{
    //public class VisualCanvasItem : ListBoxItem
    public class VisualCanvasItem : ListBoxItem
    {
        public Boolean IsDrawing
        {
            get { return (Boolean)GetValue(IsDrawingProperty); }
            set { SetValue(IsDrawingProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsDrawing.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsDrawingProperty =
            DependencyProperty.Register("IsDrawing", typeof(Boolean), typeof(VisualCanvasItem), new PropertyMetadata(false, OnIsDrawingChanged));

        protected static void OnIsDrawingChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            obj.SetValue(VisualCanvasItem.IsDrawnProperty, !(Boolean)e.NewValue);
        }

        public Boolean IsDrawn
        {
            get { return (Boolean)GetValue(IsDrawnProperty); }
            set { SetValue(IsDrawnProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsDrawn.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsDrawnProperty =
            DependencyProperty.Register("IsDrawn", typeof(Boolean), typeof(VisualCanvasItem), new PropertyMetadata(true, OnIsDrawnChanged));

        protected static void OnIsDrawnChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            obj.SetValue(VisualCanvasItem.IsDrawingProperty, !(Boolean)e.NewValue);
        }

        public VisualCanvasItem()
            : base()
        {
            this.Resources.MergedDictionaries.Add(
                new ResourceDictionary()
                {
                    Source = new Uri("pack://application:,,,/Yuhan.WPF.VisualContainer;component/Resources/VisualCanvasItem.xaml")
                });
            this.Background = new SolidColorBrush(Colors.Blue);
        }
    }
}
