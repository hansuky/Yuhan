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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Yuhan.WPF.Controls
{
    /// <summary>
    /// GuageBar.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class GaugeBar : UserControl
    {
        public String Text
        {
            get { return (String)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Value.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(String), typeof(GaugeBar), new PropertyMetadata(null));

        public Boolean ShowValue
        {
            get { return (Boolean)GetValue(ShowValueProperty); }
            set { SetValue(ShowValueProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ShowValue.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ShowValueProperty =
            DependencyProperty.Register("ShowValue", typeof(Boolean), typeof(GaugeBar), new PropertyMetadata(false));



        public Double Percentage
        {
            get { return (Double)GetValue(PercentageProperty); }
            set { SetValue(PercentageProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Percentage.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PercentageProperty =
            DependencyProperty.Register("Percentage", typeof(Double), typeof(GaugeBar), new PropertyMetadata(100.0, PercentageChanged));

        public static void PercentageChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            GaugeBar gaugeBar = sender as GaugeBar;
            gaugeBar.SetPercentageBarSize();
        }


        public GaugeBar()
        {
            InitializeComponent();
            DoubleAnimation ani = new DoubleAnimation();
            this.Loaded += GaugeBar_Loaded;
            this.SizeChanged += GaugeBar_SizeChanged;
        }

        void GaugeBar_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            this.SetPercentageBarSize();
        }

        private void SetPercentageBarSize()
        {
            this.SetPercentageBarSize(this.Percentage);
        }

        private void SetPercentageBarSize(Double percentage)
        {
            this.Bar.Width = this.ActualWidth * (percentage / 100);
        }

        void GaugeBar_Loaded(object sender, RoutedEventArgs e)
        {
            
        }
    }
}
