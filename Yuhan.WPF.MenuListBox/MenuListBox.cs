using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace Yuhan.WPF.MenuListBox
{
    public class MenuListBox : ItemsControl
    {
        public Orientation Orientation
        {
            get { return (Orientation)GetValue(OrientationProperty); }
            set { SetValue(OrientationProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Orientation.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty OrientationProperty =
            DependencyProperty.Register("Orientation", typeof(Orientation), typeof(MenuListBox), new PropertyMetadata(Orientation.Horizontal));



        public CornerRadius CornerRadius
        {
            get { return (CornerRadius)GetValue(CornerRadiusProperty); }
            set { SetValue(CornerRadiusProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CornerRadius.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CornerRadiusProperty =
            DependencyProperty.Register("CornerRadius", typeof(CornerRadius), typeof(MenuListBox), new PropertyMetadata(new CornerRadius(2)));

        public void Show()
        {
            DoubleAnimation animation = new DoubleAnimation(1, new Duration(new TimeSpan(0, 0, 0, 0, 200)));
            Storyboard.SetTargetProperty(animation, new PropertyPath(MenuListBox.OpacityProperty));
            Storyboard story = new Storyboard();
            story.Children.Add(animation);
            story.Begin(this);
        }

        public void Hide()
        {
            DoubleAnimation animation = new DoubleAnimation(0, new Duration(new TimeSpan(0, 0, 0, 0, 200)));
            Storyboard.SetTargetProperty(animation, new PropertyPath(MenuListBox.OpacityProperty));
            Storyboard story = new Storyboard();
            story.Children.Add(animation);
            story.Begin(this);
        }

        public MenuListBox()
            : base()
        {
            this.Resources.MergedDictionaries.Add(
                new ResourceDictionary() {
                    Source = new Uri("pack://application:,,,/Yuhan.WPF.MenuListBox;component/Resources/MenuListBox.xaml") 
                });
        }
    }
}
