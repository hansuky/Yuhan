using System;
using System.IO;
using System.Windows;
using System.Windows.Markup;

namespace Yuhan.WPF.CustomWindow
{
    public class WindowMinimizeButton : WindowButton
    {
        public WindowMinimizeButton()
        {
            // open resource where in XAML are defined some required stuff such as icons and colors
            Stream resourceStream = Application.GetResourceStream(new Uri("pack://application:,,,/Yuhan.WPF.CustomWindow;component/ButtonIcons.xaml")).Stream;
            ResourceDictionary resourceDictionary = (ResourceDictionary)XamlReader.Load(resourceStream);

            this.Content = resourceDictionary["WindowButtonMinimizeIcon"];
            this.ContentDisabled = resourceDictionary["WindowButtonMinimizeIconDisabled"];

            this.CornerRadius = new CornerRadius(0, 0, 0, 3);
        }
    }
}
