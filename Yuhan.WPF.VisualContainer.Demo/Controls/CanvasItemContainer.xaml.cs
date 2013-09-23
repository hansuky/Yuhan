using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Yuhan.WPF.Controls;

namespace Yuhan.WPF.VisualContainer.Demo.Controls
{
    /// <summary>
    /// Collector.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class CanvasItemContainer : Yuhan.WPF.Controls.AdornedControl
    {
        private Boolean IsSizingStart { get; set; }
        private Point StartMousePoint { get; set; }
        private Point CurrentMousePoint { get; set; }

        public Boolean IsEditable
        {
            get { return (Boolean)GetValue(IsEditableProperty); }
            set { SetValue(IsEditableProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsEditable.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsEditableProperty =
            DependencyProperty.Register("IsEditable", typeof(Boolean), typeof(CanvasItemContainer), new PropertyMetadata(false));



        public String ContainerName
        {
            get { return (String)GetValue(ContainerNameProperty); }
            set { SetValue(ContainerNameProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ContainerName.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ContainerNameProperty =
            DependencyProperty.Register("ContainerName", typeof(String), typeof(CanvasItemContainer), new PropertyMetadata(String.Empty));



        public CanvasItemContainer()
        {
            InitializeComponent();
            IsSizingStart = false;
        }

        private void Thumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            Canvas.SetLeft(this, Canvas.GetLeft(this) + e.HorizontalChange);
            Canvas.SetTop(this, Canvas.GetTop(this) + e.VerticalChange);
        }

        private void SizeBtn_MouseDown(object sender, MouseButtonEventArgs e)
        {
            IsSizingStart = true;
            StartMousePoint = e.GetPosition(sender as IInputElement);
            Mouse.Capture(sender as IInputElement);
        }

        private void SizeBtn_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if(IsSizingStart){
            IsSizingStart = false;
            Mouse.Captured.ReleaseMouseCapture();
                }
        }

        private void SizeBtn_Move(object sender, MouseEventArgs e)
        {
            if (IsSizingStart)
            {
                FrameworkElement element = sender as FrameworkElement;
                CurrentMousePoint = e.GetPosition(sender as IInputElement);
                switch (element.Tag.ToString())
                {
                    case "Left":
                        var originX = Canvas.GetLeft(this);
                        var x = originX + CurrentMousePoint.X - StartMousePoint.X;
                        var width = (Double)this.GetValue(CanvasItemContainer.ActualWidthProperty) + StartMousePoint.X - CurrentMousePoint.X;
                        if (width > 10)
                        {
                            Canvas.SetLeft(this, x);
                            this.SetCurrentValue(FrameworkElement.WidthProperty, width);
                        }
                        break;
                    case "Right":
                        width = CurrentMousePoint.X - StartMousePoint.X + (Double)this.GetValue(CanvasItemContainer.ActualWidthProperty);
                        if(width > 10)
                            this.SetCurrentValue(FrameworkElement.WidthProperty, width);
                        break;
                    case "Top":
                        var originY = Canvas.GetTop(this);
                        var height = (Double)this.GetValue(CanvasItemContainer.ActualHeightProperty) + StartMousePoint.Y - CurrentMousePoint.Y;
                        var y = originY + CurrentMousePoint.Y - StartMousePoint.Y;
                        if (height > 10)
                        {
                            Canvas.SetTop(this, y);
                            this.SetCurrentValue(FrameworkElement.HeightProperty, height);
                        }
                        break;
                    case "Bottom":
                        height = CurrentMousePoint.Y - StartMousePoint.Y + (Double)this.GetValue(CanvasItemContainer.ActualHeightProperty);
                        if(height > 10)
                            this.SetCurrentValue(FrameworkElement.HeightProperty, height);
                        break;
                    default:
                        x = Canvas.GetLeft(this) + CurrentMousePoint.X - StartMousePoint.X;
                        Canvas.SetLeft(this, x);
                        y = Canvas.GetTop(this) + CurrentMousePoint.Y - StartMousePoint.Y;
                        Canvas.SetTop(this, y);
                        break;
                }
            }
            base.OnMouseMove(e);
        }

        private void SetUpRequestBtn_Click(object sender, RoutedEventArgs e)
        {
            if (SetUpRequest != null)
                SetUpRequest(this, new EventArgs());
        }

        public event EventHandler SetUpRequest;
    }
}
