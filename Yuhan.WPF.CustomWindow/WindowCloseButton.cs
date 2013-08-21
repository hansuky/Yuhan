using System;
using System.IO;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Media;

namespace Yuhan.WPF.CustomWindow
{
    public class WindowCloseButton : WindowButton
    {
        Brush _backgroundDefaultValue;

        public override Brush BackgroundDefaultValue
        {
            get { return _backgroundDefaultValue; }
        }

        public WindowCloseButton()
        {
            this.Width = 43;
            
            // open resource where in XAML are defined some required stuff such as icons and colors
            Stream resourceStream = Application.GetResourceStream(new Uri("pack://application:,,,/Yuhan.WPF.CustomWindow;component/ButtonIcons.xaml")).Stream;
            ResourceDictionary resourceDictionary = (ResourceDictionary)XamlReader.Load(resourceStream);

            //
            // Background
            this.Background = (Brush)resourceDictionary["RedButtonBackground"];
            _backgroundDefaultValue = (Brush)resourceDictionary["RedButtonBackground"];
            
            //
            // Foreground (represents a backgroundcolor when Mouse is over)
            this.Foreground = (Brush)resourceDictionary["RedButtonMouseOverBackground"];
            
            // set icon
            this.Content = resourceDictionary["WindowButtonCloseIcon"];

            // radius
            this.CornerRadius = new CornerRadius(0, 0, 3, 0);
        }
    }
}
