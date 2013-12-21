using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

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

        public Brush BorderBrush
        {
            get { return (Brush)GetValue(BorderBrushProperty); }
            set { SetValue(BorderBrushProperty, value); }
        }

        // Using a DependencyProperty as the backing store for BorderBrush.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty BorderBrushProperty =
            DependencyProperty.Register("BorderBrush",
            typeof(Brush), typeof(GaugeBar),
            new PropertyMetadata(new SolidColorBrush(Color.FromRgb(98, 141, 182)), OnBorderBrushChanged));

        protected static void OnBorderBrushChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) { }

        public Brush TBarBackground
        {
            get { return (Brush)GetValue(TBarBackgroundProperty); }
            set { SetValue(TBarBackgroundProperty, value); }
        }

        // Using a DependencyProperty as the backing store for TBarBackground.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TBarBackgroundProperty =
            DependencyProperty.Register("TBarBackground",
            typeof(Brush), typeof(GaugeBar),
            new PropertyMetadata(new SolidColorBrush(Color.FromRgb(59, 89, 152)), OnTBarBackgroundChanged));

        protected static void OnTBarBackgroundChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) { }



        public Brush VBarBackground
        {
            get { return (Brush)GetValue(VBarBackgroundProperty); }
            set { SetValue(VBarBackgroundProperty, value); }
        }

        // Using a DependencyProperty as the backing store for VBarBackground.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty VBarBackgroundProperty =
            DependencyProperty.Register("VBarBackground",
            typeof(Brush), typeof(GaugeBar),
            new PropertyMetadata(new SolidColorBrush(Color.FromRgb(191, 19, 7)), OnVBarBackgroundChanged));

        protected static void OnVBarBackgroundChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) { }

        public GaugeBar()
        {
            InitializeComponent();
            this.Foreground = new SolidColorBrush(Colors.White);
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
