using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Yuhan.WPF.AdornerdControl.Demo
{
    /// <summary>
    /// AdvancedAdornedControlSample.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class AdvancedAdornedControlSample : Window
    {
        private static readonly double fadeOutTime = 1;
        private static readonly double fadeInTime = 0.25;

        public AdvancedAdornedControlSample()
        {
            InitializeComponent();

            closeButtonFadeoutTimer.Tick += new EventHandler(closeButtonFadeoutTimer_Tick);
            closeButtonFadeoutTimer.Interval = TimeSpan.FromSeconds(2);
        }

        enum State
        {
            Hidden,
            Visible,
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ellipse_MouseEnter(object sender, MouseEventArgs e)
        {
            bool wasFadingOut = fadeOutAnimation != null;
            fadeOutAnimation = null; // Abort fade out.

            if (closeButtonFadeoutTimer.IsEnabled)
            {
                Trace.WriteLine("ellipse_MouseEnter: Stopped fade out delay timer.");

                closeButtonFadeoutTimer.Stop();
            }

            if (!adornedControl.IsAdornerVisible)
            {
                Trace.WriteLine("ellipse_MouseEnter: Adorner hidden, fading it in.");

                adornedControl.ShowAdorner();

                DoubleAnimation doubleAnimation2 = new DoubleAnimation(1.0, new Duration(TimeSpan.FromSeconds(fadeInTime)));
                doubleAnimation2.Completed += new EventHandler(doubleAnimation2_Completed);
                adornerCanvas.BeginAnimation(Canvas.OpacityProperty, doubleAnimation2);
            }
            else if (wasFadingOut)
            {
                // Was fading out, fade back in.
                Trace.WriteLine("closeButton_MouseEnter: Was fading out, fade back in.");

                DoubleAnimation doubleAnimation = new DoubleAnimation(1.0, new Duration(TimeSpan.FromSeconds(fadeInTime)));
                adornerCanvas.BeginAnimation(Canvas.OpacityProperty, doubleAnimation);
            }
            else
            {
                adornerCanvas.BeginAnimation(Canvas.OpacityProperty, null);
            }
        }

        void doubleAnimation2_Completed(object sender, EventArgs e)
        {
            Trace.WriteLine("doubleAnimation2_Completed: Finished adorner fade in.");
        }

        private void ellipse_MouseLeave(object sender, MouseEventArgs e)
        {
            Trace.WriteLine("ellipse_MouseLeave: Started fade out delay timer.");

            closeButtonFadeoutTimer.Start();
        }

        DoubleAnimation fadeOutAnimation = null;

        void closeButtonFadeoutTimer_Tick(object sender, EventArgs e)
        {
            Trace.WriteLine("closeButtonFadeoutTimer_Tick: Fade out delay timer elapsed, starting fade out.");

            closeButtonFadeoutTimer.Stop();

            fadeOutAnimation = new DoubleAnimation(0.0, new Duration(TimeSpan.FromSeconds(fadeOutTime)));
            fadeOutAnimation.Completed += new EventHandler(doubleAnimation_Completed);
            adornerCanvas.BeginAnimation(Canvas.OpacityProperty, fadeOutAnimation);
        }

        void doubleAnimation_Completed(object sender, EventArgs e)
        {
            if (fadeOutAnimation == null)
            {
                Trace.WriteLine("doubleAnimation_Completed: Fade out aborted.");
            }
            else
            {
                Trace.WriteLine("doubleAnimation_Completed: Fade out complete, hiding the adorner.");

                adornedControl.HideAdorner();

                fadeOutAnimation = null;
            }
        }

        DispatcherTimer closeButtonFadeoutTimer = new DispatcherTimer();


        private void closeButton_Click(object sender, RoutedEventArgs e)
        {
            canvas.Children.Remove(adornedControl);
        }

        private void closeButton_MouseEnter(object sender, MouseEventArgs e)
        {
            bool wasFadingOut = fadeOutAnimation != null;
            fadeOutAnimation = null; // Abort fade out.

            if (closeButtonFadeoutTimer.IsEnabled)
            {
                Trace.WriteLine("closeButton_MouseEnter: Fade out delay timer active, stopping it.");

                closeButtonFadeoutTimer.Stop();
            }

            Trace.Assert(adornedControl.IsAdornerVisible);

            if (!adornedControl.IsAdornerVisible)
            {
                Trace.WriteLine("closeButton_MouseEnter: Adorner is hidden, showing it!");

                adornedControl.ShowAdorner();

                DoubleAnimation doubleAnimation = new DoubleAnimation(1.0, new Duration(TimeSpan.FromSeconds(fadeInTime)));
                adornerCanvas.BeginAnimation(Canvas.OpacityProperty, doubleAnimation);
            }
            else if (wasFadingOut)
            {
                // Was fading out, fade back in.
                Trace.WriteLine("closeButton_MouseEnter: Was fading out, fade back in.");

                DoubleAnimation doubleAnimation = new DoubleAnimation(1.0, new Duration(TimeSpan.FromSeconds(fadeInTime)));
                adornerCanvas.BeginAnimation(Canvas.OpacityProperty, doubleAnimation);
            }
            else
            {
                Trace.WriteLine("closeButton_MouseEnter: Adorner is not hidden, clearing animation!");

                adornerCanvas.BeginAnimation(Canvas.OpacityProperty, null);
            }
        }

        private void closeButton_MouseLeave(object sender, MouseEventArgs e)
        {
            Trace.WriteLine("closeButton_MouseLeave: Started fade out delay timer.");

            closeButtonFadeoutTimer.Start();
        }

        private void Thumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            Canvas.SetLeft(adornedControl, Canvas.GetLeft(adornedControl) + e.HorizontalChange);
            Canvas.SetTop(adornedControl, Canvas.GetTop(adornedControl) + e.VerticalChange);
        }
    }
}
